using Domain.Categories;
using Domain.Products;
using Domain.ProductTypes.Details;
using Domain.ProductTypes.Tags.TagOptions;

namespace Application.TagOptions;

public interface ITagOptionRepository
{
    Task AddAsync(TagOption option);
    Task<TagOption?> FindByIdAsync(TagOptionId id);
    Task<IEnumerable<TagOption>> FindByIds(IEnumerable<TagOptionId> ids);
    Task<IEnumerable<TagOption>> GetTagOptionsPage(TagId tagId, int offset, int pageSize);
    Task<IDictionary<TagId, List<TagOption>>> GroupByTagIdAsync(IEnumerable<TagOptionId> ids);
    Task<IEnumerable<TagOption>> FilterCategoryTagOptions(IEnumerable<TagOptionId> tagOptionIds, CategoryId categoryId);
    Task<bool> BelongsToCategory(TagOptionId tagOptionId, CategoryId categoryId);
    Task<IEnumerable<(Tag Tag, TagOption TagOption)>> GetProductConfigurationAsync(ProductId productId);
    Task<bool> UpdateAsync(TagOption option);
    Task<bool> DeleteAsync(TagOptionId id);
}
