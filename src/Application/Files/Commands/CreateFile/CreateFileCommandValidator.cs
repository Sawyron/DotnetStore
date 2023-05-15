using FluentValidation;

namespace Application.Files.Commands.CreateFile;

internal class CreateFileCommandValidator : AbstractValidator<CreateFileCommand>
{
    private static readonly string[] _allowedExtension = { ".jpeg", ".png", ".jpg" };

    public CreateFileCommandValidator()
    {
        RuleFor(command => command.Extension)
            .Must(extension => _allowedExtension.Contains(extension))
            .WithMessage($"Invalid file extension, allowed extensions: {string.Join(' ', _allowedExtension)}");
    }
}
