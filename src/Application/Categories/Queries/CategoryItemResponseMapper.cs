using Application.Categories.Queries.GetPrimaryCategoryPage;
using Domain.Categories;

namespace Application.Categories.Queries;

public class CategoryItemResponseMapper : ICategoryMapper<CategoryItemResponse>
{
    public CategoryItemResponse Map(CategoryId id, string name) => new(id.Value, name);
}
