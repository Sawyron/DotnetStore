using Domain.Categories;
using Domain.Files;
using Domain.Products;
using Domain.ProductTypes.Tags.TagOptions;
using Domain.Tags;
using Persistence.Data.Categories;
using Persistence.Data.Images;
using Persistence.Data.Products;
using Persistence.Data.TagOptions;
using Persistence.Data.Tags;

namespace Persistence.UnitTests.Data;

internal class Fixtures
{
    public Fixtures()
    {
        Categories = Enumerable.Range(1, 20)
            .Select(i => new CategoryData()
            {
                Id = new CategoryId(Guid.NewGuid()),
                Name = $"category_name_{i}",
            })
            .ToList();
        Files = Enumerable.Range(1, 250)
            .Select(i => new FileData()
            {
                Id = new FileId(Guid.NewGuid()),
                Path = $"file_path_{i}"
            })
            .ToList();
        Tags = Enumerable.Range(1, 50)
            .Select(i => new TagData()
            {
                Id = new TagId(Guid.NewGuid()),
                Category = Categories[i % Categories.Count],
                CategoryId = Categories[i % Categories.Count].Id,
                Name = $"tag_name_{i}"
            })
            .ToList();
        TagOptions = Enumerable.Range(1, 500)
            .Select(i => new TagOptionData()
            {
                Id = new TagOptionId(Guid.NewGuid()),
                Value = $"tag_option_value_{i}",
                Tag = Tags[i % Tags.Count],
                TagId = Tags[i % Tags.Count].Id
            })
            .ToList();
        Products = Enumerable.Range(1, 250)
            .Select(i => new ProductData()
            {
                Id = new ProductId(Guid.NewGuid()),
                Category = Categories[i % Categories.Count],
                CategoryId = Categories[i % Categories.Count].Id,
                Name = $"product_name_{i}",
                Description = $"product_description_{i}",
                Price = i * 1000,
                Photo = Files[i % Files.Count],
                PhotoId = Files[i % Files.Count].Id
            })
            .ToList();
        for (int i = 0; i < Products.Count; i++)
        {
            var product = Products[i];
            var options = TagOptions.Where(o => o.Tag.CategoryId == product.CategoryId)
                .DistinctBy(o => o.TagId)
                .ToList();
            product.Options = options;
        }
    }
    public List<CategoryData> Categories { get; init; }
    public List<FileData> Files { get; init; }
    public List<TagData> Tags { get; init; }
    public List<TagOptionData> TagOptions { get; init; }
    public List<ProductData> Products { get; init; }
}
