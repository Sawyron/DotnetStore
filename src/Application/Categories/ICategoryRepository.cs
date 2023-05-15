using Domain.Categories;

namespace Application.Categories;

public interface ICategoryRepository
{
    Task AddAsync(Category category, CategoryId? parentId);
    Task<Category?> FindByIdAsync(CategoryId id);
    Task<Category?> FindByNameAsync(string name);
    Task<IEnumerable<Category>> FindChildrenAsync(CategoryId categoryId);
    Task<Category?> FindParentAsync(CategoryId categoryId);
    Task<IEnumerable<Category>> GetPageAsync(int offset, int pageSize);
    Task<IEnumerable<Category>> GetAllPrimaryAsync();
    Task<bool> UpdateAsync(Category category, CategoryId? parentId);
    Task<int> UpdateParentIdAsync(CategoryId currentParentId, CategoryId updatedParentId);
    Task<bool> DeleteAsync(CategoryId categoryId);
    Task<long> CountAsync();
}
