using FluentValidation;

namespace Application.Authentication.Login;

internal class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(command => command.Email)
            .EmailAddress();
    }
}
