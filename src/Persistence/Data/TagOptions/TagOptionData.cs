using Domain.ProductTypes.Tags.TagOptions;
using Domain.Tags;
using Persistence.Data.Tags;

namespace Persistence.Data.TagOptions;

internal class TagOptionData
{
    public TagOptionId Id { get; set; } = default!;
    public string Value { get; set; } = string.Empty;
    public TagId TagId { get; set; } = default!;
    public TagData Tag { get; set; } = default!;

    public TagOption ToDomainModel() => new(Id, Value, TagId);
}
