using Domain.Categories;

namespace Persistence.Data.Categories
{
    internal class CategoryDataMapper : ICategoryMapper<CategoryData>
    {
        CategoryData ICategoryMapper<CategoryData>.Map(CategoryId id, string name)
            => new()
            {
                Id = id,
                Name = name
            };
    }
}
