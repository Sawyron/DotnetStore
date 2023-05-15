using Application.Files.Commands.CreateFile;
using FluentValidation;

namespace Application.Products.Commands.ReplacePhoto;

internal class ReplaceProductPhotoCommandValidator : AbstractValidator<ReplaceProductPhotoCommand>
{
    public ReplaceProductPhotoCommandValidator(IEnumerable<IValidator<CreateFileCommand>> fileValidators)
    {
        foreach (var fileValidator in fileValidators)
        {
            RuleFor(command => command.Photo).SetValidator(fileValidator);
        }
    }
}
