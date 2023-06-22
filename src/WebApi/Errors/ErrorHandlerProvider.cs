namespace WebApi.Errors;

public class ErrorHandlerProvider : IErrorHandlerProvider
{
    private readonly IDictionary<Type, IErrorHandler> _handlerDict;

    public ErrorHandlerProvider(IDictionary<Type, IErrorHandler> handlerDict)
    {
        _handlerDict = new Dictionary<Type, IErrorHandler>(handlerDict);
    }

    public IErrorHandler? GetErrorHandler(Type errorType)
    {
        if (_handlerDict.TryGetValue(errorType, out IErrorHandler? handler))
        {
            return handler;
        }
        return null;
    }
}
