namespace WebApi.Errors;

public interface IErrorHandlerProvider
{
    IErrorHandler? GetErrorHandler(Type errorType);
}
