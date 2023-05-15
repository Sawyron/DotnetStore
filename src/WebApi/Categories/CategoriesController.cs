using Application.Categories.Commands.CreateCategory;
using Application.Categories.Commands.DeleteCategory;
using Application.Categories.Commands.UpdateCategory;
using Application.Categories.Queries;
using Application.Categories.Queries.GetCategoryById;
using Application.Categories.Queries.GetPrimaryCategoryPage;
using Application.Core;
using Application.Tags.Commands.CreateTags;
using Application.Tags.Queries.GetCategoryTags;
using Domain.Categories;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Categories.Requests;

namespace WebApi.Categories;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ISender _sender;

    public CategoriesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(CategoryId), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CategoryId>> CreateCategory(CreateCategoryRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateCategoryCommand(request.Name, request.ParentId is null ? null : new CategoryId(request.ParentId.Value));
        CategoryId response = await _sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetCategoryById), new { id = response.Value }, response);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoryResponse>> GetCategoryById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetCategoryByIdQuery(new CategoryId(id));
        CategoryResponse response = await _sender.Send(query, cancellationToken);
        return Ok(response);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CategoryItemResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPage(int offset = 0, int pageSize = 100, CancellationToken cancellationToken = default)
    {
        var query = new GetPageQuery<CategoryItemResponse>(offset, pageSize);
        IEnumerable<CategoryItemResponse> response = await _sender.Send(query, cancellationToken);
        return Ok(response);
    }

    [HttpGet("Primary")]
    [ProducesResponseType(typeof(IEnumerable<CategoryItemResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllPrimary(CancellationToken cancellationToken)
    {
        var query = new GetPageQuery<CategoryItemResponse>(0, 100);
        var response = await _sender.Send(query, cancellationToken);
        return Ok(response);
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateCategoryRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateCategoryCommand(
            new CategoryId(id),
            request.Name,
            new CategoryId(request.ParentId));
        await _sender.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteCategoryCommand(new CategoryId(id));
        await _sender.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpGet("{id:guid}/Tags")]
    [ProducesResponseType(typeof(IEnumerable<TagResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TagResponse>>> GetCategoryTags(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetCategoryTagsQuery(new CategoryId(id));
        IEnumerable<TagResponse> response = await _sender.Send(query, cancellationToken);
        return Ok(response);
    }
}
