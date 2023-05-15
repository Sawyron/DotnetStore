using Domain.Products;
using Domain.ProductTypes.Tags.TagOptions;
using Persistence.Data.TagOptions;

namespace Persistence.Data.Products;

internal class ProductToTagOption
{
    public ProductId ProductId { get; set; } = default!;
    public ProductData Product { get; set; } = default!;
    public TagOptionId TagOptionId { get; set; } = default!;
    public TagOptionData TagOption { get; set; } = default!;
}
