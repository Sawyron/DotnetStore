using Domain.Categories;
using Domain.Files;
using Domain.Products;
using Domain.ProductTypes.Tags.TagOptions;
using Persistence.Data.Categories;
using Persistence.Data.Images;
using Persistence.Data.TagOptions;

namespace Persistence.Data.Products;

internal class ProductData
{
    public ProductId Id { get; set; } = default!;
    public string Name { get; set; } = string.Empty;
    public int Price { get; set; }
    public string Description { get; set; } = string.Empty;
    public IEnumerable<TagOptionId> OptionIds { get; set; } = default!;
    public List<TagOptionData> Options { get; set; } = default!;
    public FileId PhotoId { get; set; } = default!;
    public FileData Photo { get; set; } = default!;
    public CategoryId CategoryId { get; set; } = default!;
    public CategoryData Category { get; set; } = default!;

    public Product ToDomainModel() => new(
        Id,
        Name,
        Price,
        Description,
        Photo.ToDomainModel(),
        Options.Select(o => o.Id),
        CategoryId);
}
