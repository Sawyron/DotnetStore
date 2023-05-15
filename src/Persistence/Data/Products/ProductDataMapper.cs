using Domain.Categories;
using Domain.Files;
using Domain.Products;
using Domain.ProductTypes.Tags.TagOptions;
using Persistence.Data.Images;

namespace Persistence.Data.Products;

internal class ProductDataMapper : IProductMapper<ProductData>
{
    private readonly IFileMapper<FileData> _fileMapper;

    public ProductDataMapper(IFileMapper<FileData> fileMapper)
    {
        _fileMapper = fileMapper;
    }

    public ProductData Map(
        ProductId id,
        string name,
        int price,
        string description,
        IEnumerable<TagOptionId> configuration,
        StoredFile photo,
        CategoryId categoryId) => new()
        {
            Id = id,
            Name = name,
            Price = price,
            Description = description,
            Photo = photo.Map(_fileMapper),
            CategoryId = categoryId,
            OptionIds = configuration.ToList()
        };
}
