using Domain.ProductTypes.Tags.TagOptions;

namespace Application.Exceptions.TagOptions;

internal class TagOptionNotFoundException : ResourceNotFoundException
{
    public TagOptionNotFoundException(TagOptionId id)
        : base($"Tag optins with id '{id.Value}' is not found") { }
}
