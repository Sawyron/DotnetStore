using Application.Tags.Commands.CreateTag;
using FluentValidation;

namespace Application.Tags.Commands.CreateTags;

internal class CreateTagsCommandValidator : AbstractValidator<CreateTagsCommand>
{
    public CreateTagsCommandValidator(IEnumerable<IValidator<CreateTagCommand>> validators)
    {
        foreach (var valdator in validators.ToList())
        {
            RuleForEach(t => t.Tags).SetValidator(valdator);
        }
    }
}
