using Application.Tags;
using Domain.Categories;
using Domain.Tags;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Data.Tags;

internal class TagRepository : ITagRepository
{
    private readonly ApplicationContext _context;
    private readonly ITagMapper<TagData> _mapper;

    public TagRepository(ApplicationContext context, ITagMapper<TagData> mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task AddAsync(Tag tag)
    {
        await _context.Set<TagData>().AddAsync(tag.Map(_mapper));
    }

    public async Task AddRangeAsync(IEnumerable<Tag> tags)
    {
        await _context.Set<TagData>().AddRangeAsync(tags.Select(t => t.Map(_mapper)));
    }

    public async Task<bool> DeleteAsync(TagId id)
    {
        TagData? tag = await _context.Set<TagData>().FindAsync(id);
        if (tag is null)
        {
            return false;
        }
        _context.Set<TagData>().Remove(tag);
        return true;
    }

    public async Task<IEnumerable<Tag>> FindCategoryTags(CategoryId categoryId)
    {
        List<TagData> tags = await _context.Set<TagData>()
            .Where(t => t.CategoryId == categoryId)
            .ToListAsync();
        return tags.Select(t => t.ToDomainModel()).ToList();
    }

    public async Task<IEnumerable<Tag>> FindByIdsAsync(IEnumerable<TagId> ids)
    {
        List<TagId> idList = ids.ToList();
        List<TagData> tags = await _context.Set<TagData>()
            .Where(t => idList.Contains(t.Id))
            .ToListAsync();
        return tags.Select(t => t.ToDomainModel()).ToList();
    }

    public async Task<bool> UpdateAsync(Tag tag)
    {
        TagData updatedTag = tag.Map(_mapper);
        TagData? existingTag = await _context.Set<TagData>()
            .FindAsync(updatedTag.Id);
        if (existingTag is null)
        {
            return false;
        }
        existingTag.Name = updatedTag.Name;
        existingTag.CategoryId = updatedTag.CategoryId;
        return true;
    }

    public async Task<Tag?> FindByNameInCategory(string name, CategoryId categoryId)
    {
        TagData? tag = await _context.Set<TagData>()
            .FirstOrDefaultAsync(t => t.Name == name && t.CategoryId == categoryId);
        return tag?.ToDomainModel();
    }

    public async Task<Tag?> FindByIdAsync(TagId id)
    {
        TagData? tag = await _context.Set<TagData>()
            .FindAsync(id);
        return tag?.ToDomainModel();
    }
}
