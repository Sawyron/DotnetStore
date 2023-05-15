using Domain.ProductTypes.Tags.TagOptions;
using Domain.Tags;

namespace Application.TagOptions;

internal class TagOptionResponseMapper : ITagOptionMapper<TagOptionResponse>
{
    public TagOptionResponse Map(TagOptionId id, string value, TagId tagId)
        => new TagOptionResponse(id.Value, value);
}
