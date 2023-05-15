using Application.Exceptions;
using FluentValidation;
using MediatR;

namespace Application.Behaviours;

internal class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }
        var validatonResults = await Task.WhenAll(_validators
            .Select(validator => validator.ValidateAsync(request, cancellationToken)));
        Dictionary<string, IEnumerable<string>> errorsDictionary = validatonResults
            .SelectMany(validationResult =>
                validationResult.Errors)
            .Distinct()
            .Where(error => error is not null)
            .GroupBy(
                error => error.PropertyName,
                error => error.ErrorMessage,
                (propertyName, errorMessage) => new
                {
                    Key = propertyName,
                    Value = errorMessage
                })
            .ToDictionary(pair => pair.Key, value => value.Value);
        if (errorsDictionary.Any())
        {
            throw new RequestValidationException(errorsDictionary);
        }
        return await next();
    }
}
