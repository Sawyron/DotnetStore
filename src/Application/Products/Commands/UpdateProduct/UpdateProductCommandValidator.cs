using FluentValidation;

namespace Application.Products.Commands.UpdateProduct;

internal class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(command => command.Name)
            .NotEmpty()
            .MaximumLength(100);
        RuleFor(command => command.Price)
            .GreaterThanOrEqualTo(0);
        RuleFor(command => command.Description)
            .NotEmpty()
            .MinimumLength(512);
    }
}
