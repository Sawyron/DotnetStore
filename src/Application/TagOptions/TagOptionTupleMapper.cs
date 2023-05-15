using Domain.ProductTypes.Tags.TagOptions;
using Domain.Tags;

namespace Application.TagOptions;

internal class TagOptionTupleMapper : ITagOptionMapper<(TagOptionId, string, TagId)>
{
    public (TagOptionId, string, TagId) Map(TagOptionId id, string value, TagId tagId)
        => (id, value, tagId);
}
