using Application.Core;
using Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Errors;

public class ErrorController : ControllerBase
{
    private readonly ILogger<ErrorController> _logger;

    public ErrorController(ILogger<ErrorController> logger)
    {
        _logger = logger;
    }

    [Route("/error")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public IActionResult Error()
    {
        Exception? exception = HttpContext.Features?.Get<IExceptionHandlerFeature>()?.Error;
        _logger.LogError(exception, "{Exception}", exception?.Message);
        ActionResult actionResult = exception switch
        {
            RequestValidationException rve => ValidationProblem(rve.ToModelDictionary()),
            ServerApplicationException sae => Problem(statusCode: sae.StatusCode, title: sae.Message),
            _ => Problem(statusCode: StatusCodes.Status500InternalServerError, title: "An error occurred")
        };
        return actionResult;
    }
}
