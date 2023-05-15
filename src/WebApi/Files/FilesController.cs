using Application.Files.Queries.GetFileById;
using Domain.Files;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Files;

[Route("api/[controller]")]
[ApiController]
public class FilesController : ControllerBase
{
    private readonly ISender _sender;

    public FilesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<FileResponse>> GetFileById(Guid id, CancellationToken cancellationToken = default)
    {
        var query = new GetFileByIdQuery(new FileId(id));
        FileResponse response = await _sender.Send(query, cancellationToken);
        return File(response.Content, $"image/{response.Extension}");
    }
}
