using Domain.Categories;

namespace Persistence.Data.Categories;

internal class CategoryData
{
    public CategoryId Id { get; set; } = new CategoryId(Guid.NewGuid());
    public string Name { get; set; } = string.Empty;
    public CategoryId? ParentId { get; set; }
    public CategoryData? Parent { get; set; }
    public IEnumerable<CategoryData> Children { get; set; } = default!;

    public Category ToDomainModel() => new(Id, Name);
}
