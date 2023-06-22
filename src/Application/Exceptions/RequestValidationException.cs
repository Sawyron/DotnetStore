namespace Application.Exceptions;

public class RequestValidationException : Exception
{
    public RequestValidationException(IDictionary<string, IEnumerable<string>> errorsMap)
    {
        ErrorsMap = new Dictionary<string, IEnumerable<string>>(errorsMap);
    }

    public IDictionary<string, IEnumerable<string>> ErrorsMap { get; }
}
