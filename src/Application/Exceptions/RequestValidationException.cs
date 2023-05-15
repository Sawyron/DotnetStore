using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Application.Exceptions;

public class RequestValidationException : Exception
{
    private readonly IEnumerable<KeyValuePair<string, IEnumerable<string>>> _errorsDictionary;

    public RequestValidationException(IDictionary<string, IEnumerable<string>> errorsMap)
    {
        _errorsDictionary = new Dictionary<string, IEnumerable<string>>(errorsMap);
    }

    public ModelStateDictionary ToModelDictionary()
    {
        var stateDictionary = new ModelStateDictionary();
        foreach ((string propertyName, IEnumerable<string> errors) in _errorsDictionary)
        {
            foreach (string error in errors)
            {
                stateDictionary.AddModelError(propertyName, error);
            }
        }
        return stateDictionary;
    }
}
