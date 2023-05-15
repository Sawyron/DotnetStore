using Application.Products;
using Domain.Categories;
using Domain.Files;
using Domain.Products;
using Domain.ProductTypes.Tags.TagOptions;
using Microsoft.EntityFrameworkCore;
using Persistence.Data.TagOptions;

namespace Persistence.Data.Products;

internal class ProductRepository : IProductRepository
{
    private readonly ApplicationContext _context;
    private readonly IProductMapper<ProductData> _dataMapper;

    public ProductRepository(ApplicationContext context, IProductMapper<ProductData> mapper)
    {
        _context = context;
        _dataMapper = mapper;
    }

    public async Task AddAsync(Product product)
    {
        ProductData productData = product.Map(_dataMapper);
        var options = await _context.Set<TagOptionData>()
            .Where(o => productData.OptionIds.Contains(o.Id))
            .ToListAsync();
        productData.Options = options;
        await _context.Set<ProductData>().AddAsync(productData);
    }

    public async Task<long> CountAsync()
    {
        return await _context.Set<ProductData>().LongCountAsync();
    }

    public async Task<bool> DeleteAsync(ProductId id)
    {
        ProductData? product = await _context.Set<ProductData>().FindAsync(id);
        if (product is null)
        {
            return false;
        }
        _context.Set<ProductData>().Remove(product);
        return true;
    }

    public async Task<Product?> FindByIdAsync(ProductId id)
    {
        ProductData? product = await _context.Set<ProductData>()
            .AsNoTracking()
            .Include(p => p.Options)
            .Include(p => p.Photo)
            .FirstOrDefaultAsync(p => p.Id == id);
        return product?.ToDomainModel();
    }

    public async Task<IEnumerable<Product>> GetCategoryProductsPageAsync(int offset, int pageSize, CategoryId categoryId)
    {
        var products = await _context.Set<ProductData>()
            .AsNoTracking()
            .Include(p => p.Options)
            .Include(p => p.Photo)
            .Where(p => p.CategoryId == categoryId)
            .OrderBy(p => p.Name)
            .Skip(offset)
            .Take(pageSize)
            .ToListAsync();
        return products.Select(p => p.ToDomainModel()).ToList();
    }

    public async Task<bool> UpdateAsync(Product product)
    {
        ProductData updatedProduct = product.Map(_dataMapper);
        ProductData? existingProduct = await _context.Set<ProductData>()
            .FirstOrDefaultAsync(p => p.Id == updatedProduct.Id);
        if (existingProduct is null)
        {
            return false;
        }
        existingProduct.Name = updatedProduct.Name;
        existingProduct.Price = updatedProduct.Price;
        existingProduct.Description = updatedProduct.Description;
        return true;
    }

    public async Task<bool> UpdateConfigurationAsync(ProductId productId, IEnumerable<TagOptionId> tagOptionIds)
    {
        var product = await _context.Set<ProductData>()
            .Include(p => p.Options)
            .FirstOrDefaultAsync(p => p.Id == productId);
        if (product is null)
        {
            return false;
        }
        var options = await _context.Set<TagOptionData>()
            .Where(o => tagOptionIds.Contains(o.Id))
            .ToListAsync();
        product.Options = options;
        return true;
    }

    public async Task<bool> UpdatePhoto(ProductId id, FileId fileId)
    {
        ProductData? productData = await _context.Set<ProductData>()
            .FindAsync(id);
        if (productData is null)
        {
            return false;
        }
        productData.PhotoId = fileId;
        return true;
    }
}
