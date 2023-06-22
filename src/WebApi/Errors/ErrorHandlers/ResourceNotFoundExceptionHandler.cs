using Application.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Errors.ErrorHandlers;

public class ResourceNotFoundExceptionHandler : ErrorHandlerBase<ResourceNotFoundException>
{
    protected override IActionResult HandleExceprtion(ResourceNotFoundException exception, ControllerBase controller)
        => controller.NotFound(exception.Message);
}
