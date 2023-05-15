using FluentValidation;

namespace Application.Categories.Commands.CreateCategory;

internal class CreateCategoryValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryValidator()
    {
        RuleFor(c => c.Name)
            .MinimumLength(3)
            .MaximumLength(100);
    }
}
