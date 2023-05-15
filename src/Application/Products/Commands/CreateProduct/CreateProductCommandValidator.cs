using Application.Files.Commands.CreateFile;
using FluentValidation;

namespace Application.Products.Commands.CreateProduct;

internal class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator(IEnumerable<IValidator<CreateFileCommand>> fileValidators)
    {
        RuleFor(command => command.Price)
            .GreaterThan(0);
        RuleFor(command => command.Name)
            .NotEmpty()
            .MaximumLength(100);
        RuleFor(command => command.Description)
            .NotEmpty()
            .MaximumLength(512);
        foreach (var fileValidator in fileValidators)
        {
            RuleFor(command => command.Photo).SetValidator(fileValidator);
        }
    }
}
