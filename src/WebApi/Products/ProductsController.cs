using Application.Files.Commands.CreateFile;
using Application.Products;
using Application.Products.Commands.AddTagOption;
using Application.Products.Commands.CreateProduct;
using Application.Products.Commands.DeleteProduct;
using Application.Products.Commands.RemoveTagOption;
using Application.Products.Commands.ReplacePhoto;
using Application.Products.Commands.UpdateProduct;
using Application.Products.Queries.GetById;
using Application.Products.Queries.GetPage;
using Domain.Categories;
using Domain.Files;
using Domain.Products;
using Domain.ProductTypes.Tags.TagOptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Files;
using WebApi.Products.Requests;

namespace WebApi.Products;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly ISender _sender;

    public ProductsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ProductId), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateProduct([FromForm] CreateProductRequest request, CancellationToken cancellationToken)
    {
        string extension = Path.GetExtension(request.Photo.FileName);
        using var memoryStram = new MemoryStream();
        await request.Photo.CopyToAsync(memoryStram, cancellationToken);
        var command = new CreateProductCommand(
            request.Name,
            request.Price,
            request.Description,
            request.Configuration.Select(id => new TagOptionId(id)),
            new CreateFileCommand(memoryStram.ToArray(), extension),
            new CategoryId(request.CategoryId));
        ProductId response = await _sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetProductById), new { Id = response.Value }, response);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProductById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetProductByIdQuery(new ProductId(id),
            CreateProductImageLink);
        ProductResponse response = await _sender.Send(query, cancellationToken);
        return Ok(response);
    }


    [HttpGet("Category/{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<ProductItemResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPageByCategoryId(Guid id, int offset = 0, int pageSize = 100, CancellationToken cancellationToken = default)
    {
        var query = new GetProductPageByCategoryIdQuery(
            new CategoryId(id),
            offset,
            pageSize,
            CreateProductImageLink);
        IEnumerable<ProductItemResponse> response = await _sender.Send(query, cancellationToken);
        return Ok(response);
    }

    [HttpPatch("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, UpdateProductRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateProductCommand(
            new ProductId(id),
            request.Name,
            request.Price,
            request.Description);
        await _sender.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpPatch("Image/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ReplacePhoto(Guid id, IFormFile file, CancellationToken cancellationToken)
    {
        string extension = Path.GetExtension(file.FileName);
        using var ms = new MemoryStream();
        await file.CopyToAsync(ms, cancellationToken);
        var command = new ReplaceProductPhotoCommand(
            new ProductId(id),
            new CreateFileCommand(ms.ToArray(), extension));
        await _sender.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteProductCommand(new ProductId(id));
        await _sender.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:guid}/TagOptions")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddTagOption(Guid id, AddTagOptionToProductRequest request, CancellationToken cancellationToken)
    {
        var command = new AddTagOptionToProductCommand(new ProductId(id), new TagOptionId(request.TagOptionId));
        await _sender.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}/TagOptions")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveTagOption(Guid id, RemoveTagOptionFromProductRequest request, CancellationToken cancellationToken)
    {
        var command = new RemoveTagOptionFromProductCommand(new ProductId(id), new TagOptionId(request.TagOptionId));
        await _sender.Send(command, cancellationToken);
        return NoContent();
    }

    private string CreateProductImageLink(FileId id) => Url.ActionLink(
                controller: nameof(FilesController).Replace("Controller", string.Empty),
                action: nameof(FilesController.GetFileById),
                values: new { Id = id.Value }) ?? string.Empty;
}
