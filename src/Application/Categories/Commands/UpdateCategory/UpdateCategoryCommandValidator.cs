using FluentValidation;

namespace Application.Categories.Commands.UpdateCategory;

internal class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryCommandValidator()
    {
        RuleFor(command => command.Name)
            .MaximumLength(255);
    }
}
