using Domain.Categories;
using Domain.Files;
using Domain.Products;
using Domain.ProductTypes.Tags.TagOptions;

namespace Application.Products.Commands.AddTagOption;

internal class ProductCategoryIdMapper : IProductMapper<CategoryId>
{
    public CategoryId Map(
        ProductId id,
        string name,
        int price,
        string description,
        IEnumerable<TagOptionId> configuration,
        StoredFile photo,
        CategoryId categoryId) => categoryId;
}
