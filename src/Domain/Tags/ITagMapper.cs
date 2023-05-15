using Domain.Categories;

namespace Domain.Tags;

public interface ITagMapper<T>
{
    T Map(TagId id, string name, CategoryId categoryId);
}
