namespace Domain.Categories;

public sealed class Category
{
    private readonly CategoryId _id;
    private readonly string _name;

    public Category(CategoryId id, string name)
    {
        _id = id;
        _name = name;
    }

    public T Map<T>(ICategoryMapper<T> mapper) => mapper.Map(_id, _name);
}
