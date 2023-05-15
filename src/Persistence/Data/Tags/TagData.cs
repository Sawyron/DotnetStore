using Domain.Categories;
using Domain.Tags;
using Persistence.Data.Categories;
using Persistence.Data.TagOptions;

namespace Persistence.Data.Tags;

internal class TagData
{
    public TagId Id { get; set; } = default!;
    public string Name { get; set; } = string.Empty;
    public CategoryId CategoryId { get; set; } = default!;
    public CategoryData Category { get; set; } = default!;
    public IEnumerable<TagOptionData> Options { get; set; } = default!;

    public Tag ToDomainModel() => new(Id, Name, CategoryId);
}
