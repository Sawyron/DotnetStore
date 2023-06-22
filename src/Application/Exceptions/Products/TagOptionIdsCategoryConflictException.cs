using Domain.Categories;
using Domain.ProductTypes.Tags.TagOptions;

namespace Application.Exceptions.Products;

internal class TagOptionIdsCategoryConflictException : ResourceStateConflictException
{
    public TagOptionIdsCategoryConflictException(IEnumerable<TagOptionId> tagOptionIds, CategoryId categoryId)
        : base(
            $"Id's :\n{string.Join('\n', tagOptionIds.Select(id => id.Value))} do not belong to category with id {categoryId.Value}")
    { }
}
