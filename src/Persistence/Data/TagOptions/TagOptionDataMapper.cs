using Domain.ProductTypes.Tags.TagOptions;
using Domain.Tags;

namespace Persistence.Data.TagOptions;

internal class TagOptionDataMapper : ITagOptionMapper<TagOptionData>
{
    public TagOptionData Map(TagOptionId id, string value, TagId tagId)
        => new()
        {
            Id = id,
            TagId = tagId,
            Value = value
        };
}
