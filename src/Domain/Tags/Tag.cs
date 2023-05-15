using Domain.Categories;

namespace Domain.ProductTypes.Details;

public class Tag
{
    private readonly TagId _id;
    private readonly string _name;
    private readonly CategoryId _categoryId;

    public Tag(TagId id, string name, CategoryId categoryId)
    {
        _id = id;
        _name = name;
        _categoryId = categoryId;
    }

    public T Map<T>(ITagMapper<T> mapper) => mapper.Map(_id, _name, _categoryId);
}
