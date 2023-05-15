using Application.TagOptions;
using Application.TagOptions.Queries.GetPageByTagId;
using Application.Tags.Commands.CreateTag;
using Application.Tags.Commands.DeleteTag;
using Application.Tags.Commands.UpdateTag;
using Domain.Categories;
using Domain.Tags;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Tags.Requests;

namespace WebApi.Tags;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class TagsController : ControllerBase
{
    private readonly ISender _sender;

    public TagsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    [ProducesResponseType(typeof(TagId), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<TagId>> CreateTag(CreateTagRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateTagCommand(request.Name, new CategoryId(request.CategoryId));
        TagId response = await _sender.Send(command, cancellationToken);
        return Ok(response);
    }

    [HttpGet("{id:guid}/Options")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<TagOptionResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTagOptionsByTagId(Guid id, int offset = 0, int pageSize = 100, CancellationToken cancellationToken = default)
    {
        var query = new GetTagOptionsPageByTagIdQuery(offset, pageSize, new TagId(id));
        IEnumerable<TagOptionResponse> response = await _sender.Send(query, cancellationToken);
        return Ok(response);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, UpdateTagRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateTagCommand(new TagId(id), request.Name, new CategoryId(request.CategoryId));
        await _sender.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteTagCommand(new TagId(id));
        await _sender.Send(command, cancellationToken);
        return NoContent();
    }
}
