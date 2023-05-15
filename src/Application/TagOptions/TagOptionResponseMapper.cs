using Domain.ProductTypes.Details;
using Domain.ProductTypes.Tags.TagOptions;

namespace Application.TagOptions;

internal class TagOptionResponseMapper : ITagOptionMapper<TagOptionResponse>
{
    public TagOptionResponse Map(TagOptionId id, string value, TagId tagId)
        => new TagOptionResponse(id.Value, value);
}
