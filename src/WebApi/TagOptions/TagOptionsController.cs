using Application.TagOptions.Commands.CreateTagOption;
using Application.TagOptions.Commands.DeleteTagOption;
using Application.TagOptions.Commands.UpdateTagOption;
using Domain.ProductTypes.Tags.TagOptions;
using Domain.Tags;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.TagOptions.Requests;

namespace WebApi.TagOptions;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class TagOptionsController : ControllerBase
{
    private readonly ISender _sender;

    public TagOptionsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    [ProducesResponseType(typeof(TagOptionId), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<TagOptionId>> CreateTagOption(CreateTagOptionRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateTagOptionCommand(request.Value, new TagId(request.TagId));
        TagOptionId response = await _sender.Send(command, cancellationToken);
        return Ok(response);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, UpdateTagOptionRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateTagOptionCommand(new TagOptionId(id), request.Value, new TagId(request.TagId));
        await _sender.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var comand = new DeleteTagOptionCommand(new TagOptionId(id));
        await _sender.Send(comand, cancellationToken);
        return NoContent();
    }
}
