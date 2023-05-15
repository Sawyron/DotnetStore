using Domain.Categories;
using Domain.Files;
using Domain.Products;
using Domain.ProductTypes.Tags.TagOptions;

namespace Application.Products.Queries.GetById;

internal class ProductFileIdMapper : IProductMapper<FileId>
{
    private readonly IFileMapper<FileId> _fileMapper;

    public ProductFileIdMapper(IFileMapper<FileId> fileMapper)
    {
        _fileMapper = fileMapper;
    }

    public FileId Map(ProductId id,
                      string name,
                      int price,
                      string description,
                      IEnumerable<TagOptionId> configuration,
                      StoredFile photo,
                      CategoryId categoryId) => photo.Map(_fileMapper);

}
