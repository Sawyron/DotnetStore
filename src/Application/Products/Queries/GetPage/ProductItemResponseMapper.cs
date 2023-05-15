using Domain.Categories;
using Domain.Files;
using Domain.Products;
using Domain.ProductTypes.Tags.TagOptions;

namespace Application.Products.Queries.GetPage;

internal class ProductItemResponseMapper : IProductMapper<ProductItemResponse>
{
    public ProductItemResponse Map(
        ProductId id,
        string name,
        int price,
        string description,
        IEnumerable<TagOptionId> configuration,
        StoredFile photo,
        CategoryId categoryId) => new(id.Value, name, price, description, string.Empty, categoryId.Value);
}
