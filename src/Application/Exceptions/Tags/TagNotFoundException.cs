using Domain.Tags;

namespace Application.Exceptions.Tags;

internal class TagNotFoundException : ResourceNotFoundException
{
    public TagNotFoundException(TagId tagId)
        : base($"Tag with id '{tagId.Value}' is not found") { }
}
