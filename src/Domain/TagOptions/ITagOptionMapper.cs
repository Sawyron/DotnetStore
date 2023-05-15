using Domain.ProductTypes.Details;

namespace Domain.ProductTypes.Tags.TagOptions;

public interface ITagOptionMapper<T>
{
    T Map(TagOptionId id, string value, TagId tagId);
}
