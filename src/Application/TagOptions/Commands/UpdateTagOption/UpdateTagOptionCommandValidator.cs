using FluentValidation;

namespace Application.TagOptions.Commands.UpdateTagOption;

internal class UpdateTagOptionCommandValidator : AbstractValidator<UpdateTagOptionCommand>
{
    public UpdateTagOptionCommandValidator()
    {
        RuleFor(command => command.Value)
            .NotEmpty()
            .MaximumLength(255);
    }
}
