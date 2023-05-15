using Application.TagOptions;
using Domain.Categories;
using Domain.Products;
using Domain.ProductTypes.Details;
using Domain.ProductTypes.Tags.TagOptions;
using Microsoft.EntityFrameworkCore;
using Persistence.Data.Products;

namespace Persistence.Data.TagOptions;

internal class TagOptionRepository : ITagOptionRepository
{
    private readonly ApplicationContext _context;
    private readonly ITagOptionMapper<TagOptionData> _mapper;

    public TagOptionRepository(ApplicationContext context, ITagOptionMapper<TagOptionData> mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task AddAsync(TagOption option)
    {
        await _context.Set<TagOptionData>().AddAsync(option.Map(_mapper));
    }

    public async Task<bool> BelongsToCategory(TagOptionId tagOptionId, CategoryId categoryId)
    {
        TagOptionData? tag = await _context.Set<TagOptionData>()
            .AsNoTracking()
            .Include(o => o.Tag)
            .FirstOrDefaultAsync(o => o.Id == tagOptionId);
        return tag?.Tag?.CategoryId == categoryId;
    }

    public async Task<bool> DeleteAsync(TagOptionId id)
    {
        TagOptionData? option = await _context.Set<TagOptionData>()
            .FindAsync(id);
        if (option is null)
        {
            return false;
        }
        _context.Set<TagOptionData>().Remove(option);
        return true;
    }

    public async Task<IEnumerable<TagOption>> FilterCategoryTagOptions(IEnumerable<TagOptionId> tagOptionIds, CategoryId categoryId)
    {
        var options = await _context.Set<TagOptionData>()
            .AsNoTracking()
            .Include(o => o.Tag)
            .Where(o => o.Tag.CategoryId == categoryId)
            .Where(o => tagOptionIds.Contains(o.Id))
            .ToListAsync();
        return options.Select(o => o.ToDomainModel());
    }

    public async Task<TagOption?> FindByIdAsync(TagOptionId id)
    {
        var option = await _context.Set<TagOptionData>()
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == id);
        return option?.ToDomainModel();
    }

    public async Task<IEnumerable<TagOption>> FindByIds(IEnumerable<TagOptionId> ids)
    {
        List<TagOptionData> options = await _context.Set<TagOptionData>()
            .AsNoTracking()
            .Where(t => ids.Contains(t.Id))
            .ToListAsync();
        return options.Select(o => o.ToDomainModel()).ToList();
    }

    public async Task<IEnumerable<(Tag Tag, TagOption TagOption)>> GetProductConfigurationAsync(ProductId productId)
    {
        List<TagOptionData> options = await _context.Set<ProductData>()
            .AsNoTracking()
            .Where(p => p.Id == productId)
            .Include(p => p.Options)
            .SelectMany(p => p.Options)
            .Include(o => o.Tag)
            .ToListAsync();
        return options
            .OrderBy(o => o.Tag.Name)
            .Select(o => (o.Tag.ToDomainModel(), o.ToDomainModel()))
            .ToList();
    }

    public async Task<IEnumerable<TagOption>> GetTagOptionsPage(TagId tagId, int offset, int pageSize)
    {
        List<TagOptionData> tags = await _context.Set<TagOptionData>()
            .AsNoTracking()
            .Where(t => t.TagId == tagId)
            .OrderBy(t => t.Value)
            .Skip(offset)
            .Take(pageSize)
            .ToListAsync();
        return tags.Select(t => t.ToDomainModel()).ToList();
    }

    public async Task<IDictionary<TagId, List<TagOption>>> GroupByTagIdAsync(IEnumerable<TagOptionId> ids)
    {
        return await _context.Set<TagOptionData>()
            .AsNoTracking()
            .Where(o => ids.Contains(o.Id))
            .GroupBy(o => o.TagId)
            .ToDictionaryAsync(g => g.Key, g => g.Select(o => o.ToDomainModel()).ToList());
    }

    public async Task<bool> UpdateAsync(TagOption option)
    {
        TagOptionData updatedOption = option.Map(_mapper);
        TagOptionData? existingOption = await _context.Set<TagOptionData>()
            .FindAsync(updatedOption.Id);
        if (existingOption is null)
        {
            return false;
        }
        existingOption.Value = updatedOption.Value;
        existingOption.TagId = updatedOption.TagId;
        return true;
    }
}
