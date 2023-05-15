using FluentValidation;

namespace Application.Tags.Commands.CreateTag;

internal class CreateTagCommandValidator : AbstractValidator<CreateTagCommand>
{
    public CreateTagCommandValidator()
    {
        RuleFor(t => t.Name)
            .NotEmpty()
            .MaximumLength(100);
    }
}
