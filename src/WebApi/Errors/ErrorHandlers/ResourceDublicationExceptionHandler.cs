using Application.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Errors.ErrorHandlers;

public class ResourceDublicationExceptionHandler : ErrorHandlerBase<ResourceDublicationException>
{
    protected override IActionResult HandleExceprtion(ResourceDublicationException exception, ControllerBase controller)
        => controller.Conflict(exception.Message);
}
