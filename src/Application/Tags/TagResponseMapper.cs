using Application.Tags.Commands.CreateTags;
using Domain.Categories;
using Domain.ProductTypes.Details;

namespace Application.Tags;

internal class TagResponseMapper : ITagMapper<TagResponse>
{
    public TagResponse Map(TagId id, string name, CategoryId categoryId) =>
        new(id.Value, name);
}
