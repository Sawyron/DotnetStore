using Microsoft.AspNetCore.Mvc;

namespace WebApi.Errors.ErrorHandlers;

public abstract class ErrorHandlerBase<T> : IErrorHandler
    where T : Exception
{
    public bool CanHandle(Exception exception) => exception is T;

    public IActionResult Handle(Exception exception, ControllerBase controller)
    {
        if (exception is T correctTypeException)
        {
            return HandleExceprtion(correctTypeException, controller);
        }
        throw new ArgumentException($"{exception.GetType()} does not match {typeof(T)}");
    }
    protected abstract IActionResult HandleExceprtion(T exception, ControllerBase controller);
}
