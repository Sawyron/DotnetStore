using Domain.ProductTypes.Details;

namespace Domain.ProductTypes.Tags.TagOptions;

public class TagOption
{
    private readonly TagOptionId _id;
    private readonly string _value;
    private readonly TagId _tagId;

    public TagOption(TagOptionId id, string value, TagId tagId)
    {
        _id = id;
        _value = value;
        _tagId = tagId;
    }

    public T Map<T>(ITagOptionMapper<T> mapper) => mapper.Map(_id, _value, _tagId);
}
