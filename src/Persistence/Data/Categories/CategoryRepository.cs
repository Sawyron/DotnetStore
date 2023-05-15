using Application.Categories;
using Domain.Categories;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Data.Categories;

internal class CategoryRepository : ICategoryRepository
{
    private readonly ApplicationContext _context;
    private readonly ICategoryMapper<CategoryData> _mapper;

    public CategoryRepository(ApplicationContext context, ICategoryMapper<CategoryData> mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task AddAsync(Category category, CategoryId? parentId)
    {
        CategoryData entity = category.Map(_mapper);
        entity.ParentId = parentId;
        await _context.Set<CategoryData>().AddAsync(entity);
    }

    public Task<long> CountAsync()
    {
        return _context.Set<CategoryData>().LongCountAsync();
    }

    public async Task<bool> DeleteAsync(CategoryId categoryId)
    {
        CategoryData? category = await _context.Set<CategoryData>()
            .FirstOrDefaultAsync(c => c.Id == categoryId);
        if (category is null)
        {
            return false;
        }
        _context.Set<CategoryData>().Remove(category);
        return true;
    }

    public async Task<Category?> FindByIdAsync(CategoryId id)
    {
        var category = await _context.Set<CategoryData>()
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);
        return category?.ToDomainModel();
    }

    public async Task<Category?> FindByNameAsync(string name)
    {
        var category = await _context.Set<CategoryData>()
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Name == name);
        return category?.ToDomainModel();
    }

    public async Task<IEnumerable<Category>> FindChildrenAsync(CategoryId categoryId)
    {
        var categories = await _context.Set<CategoryData>()
            .AsNoTracking()
            .Where(c => c.ParentId == categoryId)
            .ToListAsync();
        return categories.Select(c => c.ToDomainModel()).ToList();
    }

    public async Task<Category?> FindParentAsync(CategoryId categoryId)
    {
        CategoryData? category = await _context.Set<CategoryData>()
            .AsNoTracking()
            .Include(c => c.Parent)
            .FirstOrDefaultAsync(c => c.Id == categoryId);
        return category?.Parent?.ToDomainModel();
    }

    public async Task<IEnumerable<Category>> GetAllPrimaryAsync()
    {
        var primaries = await _context.Set<CategoryData>()
            .Where(c => c.ParentId == null)
            .ToListAsync();
        return primaries.Select(c => c.ToDomainModel()).ToList();
    }

    public async Task<IEnumerable<Category>> GetPageAsync(int offset, int pageSize)
    {
        return await _context.Set<CategoryData>()
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Skip(offset)
            .Take(pageSize)
            .Select(category => category.ToDomainModel())
            .ToListAsync();
    }

    public async Task<bool> UpdateAsync(Category category, CategoryId? parentId)
    {
        CategoryData updatedCategory = category.Map(_mapper);
        CategoryData? existingCategory = await _context.Set<CategoryData>()
            .FindAsync(updatedCategory.Id);
        if (existingCategory is null)
        {
            return false;
        }
        existingCategory.Name = updatedCategory.Name;
        existingCategory.ParentId = parentId;
        return true;
    }

    public async Task<int> UpdateParentIdAsync(CategoryId currentParentId, CategoryId updatedParentId)
    {
        return await _context.Set<CategoryData>()
            .Where(c => c.ParentId == currentParentId)
            .ExecuteUpdateAsync(update =>
                update.SetProperty(c => c.ParentId, updatedParentId));
    }
}
