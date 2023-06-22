using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Errors;

public class ErrorController : ControllerBase
{
    private readonly ILogger<ErrorController> _logger;
    private readonly IErrorHandler _errorHandler;

    public ErrorController(ILogger<ErrorController> logger, IErrorHandler errorHandler)
    {
        _logger = logger;
        _errorHandler = errorHandler;
    }

    [Route("/error")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public IActionResult Error()
    {
        Exception? exception = HttpContext.Features?.Get<IExceptionHandlerFeature>()?.Error;
        _logger.LogError(exception, "{Exception}", exception?.Message);
        IActionResult actionResult;
        if (_errorHandler.CanHandle(exception!))
        {
            actionResult = _errorHandler.Handle(exception!, this);
        }
        else
        {
            actionResult = Problem(title: "Internal server error", statusCode: StatusCodes.Status500InternalServerError);
        }
        return actionResult;
    }
}
