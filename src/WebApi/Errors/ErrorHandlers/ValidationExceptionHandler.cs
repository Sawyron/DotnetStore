using Application.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace WebApi.Errors.ErrorHandlers;

public class ValidationExceptionHandler : ErrorHandlerBase<RequestValidationException>
{
    protected override IActionResult HandleExceprtion(RequestValidationException exception, ControllerBase controller)
    {
        var modelErros = new ModelStateDictionary();
        foreach ((string propertyName, IEnumerable<string> erros) in exception.ErrorsMap)
        {
            foreach (string error in erros)
            {
                modelErros.AddModelError(propertyName, error);
            }
        }
        return controller.ValidationProblem(modelErros);
    }
}
