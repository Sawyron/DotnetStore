using Domain.Categories;
using Domain.Tags;

namespace Persistence.Data.Tags;

internal class TagDataMapper : ITagMapper<TagData>
{
    public TagData Map(TagId id, string name, CategoryId categoryId)
        => new()
        {
            Id = id,
            Name = name,
            CategoryId = categoryId
        };
}
