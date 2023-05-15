using Domain.ProductTypes.Details;
using Domain.ProductTypes.Tags.TagOptions;

namespace Application.TagOptions;

internal class TagOptionTupleMapper : ITagOptionMapper<(TagOptionId, string, TagId)>
{
    public (TagOptionId, string, TagId) Map(TagOptionId id, string value, TagId tagId)
        => (id, value, tagId);
}
