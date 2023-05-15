using Domain.Categories;

namespace Application.Categories.Commands.CreateCategory;

internal class CategoryIdMapper : ICategoryMapper<CategoryId>
{
    public CategoryId Map(CategoryId id, string name) => id;
}
