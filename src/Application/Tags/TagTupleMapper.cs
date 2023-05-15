using Domain.Categories;
using Domain.ProductTypes.Details;

namespace Application.Tags;

internal class TagTupleMapper : ITagMapper<(TagId, string, CategoryId)>
{
    public (TagId, string, CategoryId) Map(TagId id, string name, CategoryId categoryId)
        => (id, name, categoryId);
}
