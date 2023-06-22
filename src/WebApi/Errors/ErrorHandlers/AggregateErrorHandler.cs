using Microsoft.AspNetCore.Mvc;

namespace WebApi.Errors.ErrorHandlers;

public class AggregateErrorHandler : IErrorHandler
{
    private readonly List<IErrorHandler> _handlers;

    public AggregateErrorHandler(IEnumerable<IErrorHandler> handlers)
    {
        _handlers = new List<IErrorHandler>(handlers);
    }

    public bool CanHandle(Exception exception)
        => _handlers.Any(h => h.CanHandle(exception));

    public IActionResult Handle(Exception exception, ControllerBase controller)
    {
        IErrorHandler? handler = _handlers.FirstOrDefault(h => h.CanHandle(exception));
        return handler is null
            ? throw new InvalidOperationException($"Can not handle exeption of type '{exception.GetType()}'")
            : handler.Handle(exception, controller);
    }
}
