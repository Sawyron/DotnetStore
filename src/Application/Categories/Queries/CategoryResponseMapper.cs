using Application.Categories.Queries.GetPrimaryCategoryPage;
using Domain.Categories;

namespace Application.Categories.Queries;

internal class CategoryResponseMapper : ICategoryMapper<CategoryResponse>
{
    public CategoryResponse Map(CategoryId id, string name) =>
        new(id.Value, name, Array.Empty<CategoryItemResponse>());
}
