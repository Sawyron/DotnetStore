using Application.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Errors.ErrorHandlers;

public class ResourceStateConflictExceptionHandler : ErrorHandlerBase<ResourceStateConflictException>
{
    protected override IActionResult HandleExceprtion(ResourceStateConflictException exception, ControllerBase controller)
        => controller.Conflict(exception.Message);
}
