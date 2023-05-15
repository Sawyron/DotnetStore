using Domain.Categories;
using Domain.Tags;

namespace Application.Tags;

public interface ITagRepository
{
    Task AddAsync(Tag tag);
    Task AddRangeAsync(IEnumerable<Tag> tags);
    Task<Tag?> FindByIdAsync(TagId id);
    Task<Tag?> FindByNameInCategory(string name, CategoryId categoryId);
    Task<IEnumerable<Tag>> FindByIdsAsync(IEnumerable<TagId> ids);
    Task<IEnumerable<Tag>> FindCategoryTags(CategoryId categoryId);
    Task<bool> DeleteAsync(TagId id);
    Task<bool> UpdateAsync(Tag tag);
}
