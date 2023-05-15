using Domain.ProductTypes.Details;
using Domain.ProductTypes.Tags.TagOptions;

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
