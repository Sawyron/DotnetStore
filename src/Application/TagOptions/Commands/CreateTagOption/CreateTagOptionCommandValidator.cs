using FluentValidation;

namespace Application.TagOptions.Commands.CreateTagOption;

internal class CreateTagOptionCommandValidator : AbstractValidator<CreateTagOptionCommand>
{
    public CreateTagOptionCommandValidator()
    {
        RuleFor(c => c.Value)
            .NotEmpty()
            .MaximumLength(255);
    }
}
