using Microsoft.AspNetCore.Mvc;

namespace WebApi.Errors;

public interface IErrorHandler
{
    IActionResult Handle(Exception exception, ControllerBase controller);
    bool CanHandle(Exception exception);
}
