using Domain.Categories;
using Domain.Files;
using Domain.Products;
using Domain.ProductTypes.Tags.TagOptions;

namespace Application.Products.Queries.GetById;

internal class ProductResponseMapper : IProductMapper<ProductResponse>
{

    public ProductResponse Map(
        ProductId id,
        string name,
        int price,
        string description,
        IEnumerable<TagOptionId> configuration,
        StoredFile photo,
        CategoryId categoryId) => new ProductResponse(
            id.Value,
            name,
            price,
            description,
            string.Empty,
            Array.Empty<ProductTagOptionResponse>(),
            categoryId.Value);
}
