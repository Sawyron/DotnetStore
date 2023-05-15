using Domain.Categories;
using Domain.Files;
using Domain.Products;
using Domain.ProductTypes.Tags.TagOptions;

namespace Application.Products;

public interface IProductRepository
{
    Task AddAsync(Product product);
    Task<Product?> FindByIdAsync(ProductId id);
    Task<IEnumerable<Product>> GetCategoryProductsPageAsync(int offset, int pageSize, CategoryId categoryId);
    Task<bool> UpdateAsync(Product product);
    Task<bool> UpdatePhoto(ProductId id, FileId fileId);
    Task<bool> UpdateConfigurationAsync(ProductId productId, IEnumerable<TagOptionId> tagOptionIds);
    Task<bool> DeleteAsync(ProductId id);
    Task<long> CountAsync();
}
