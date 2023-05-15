using Domain.Categories;
using Domain.Files;
using Domain.ProductTypes.Tags.TagOptions;

namespace Domain.Products;

public interface IProductMapper<T>
{
    T Map(
        ProductId id,
        string name,
        int price,
        string description,
        IEnumerable<TagOptionId> configuration,
        StoredFile photo,
        CategoryId categoryId);
}
