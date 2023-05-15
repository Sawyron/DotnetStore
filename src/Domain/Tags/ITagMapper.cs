using Domain.Categories;

namespace Domain.ProductTypes.Details;

public interface ITagMapper<T>
{
    T Map(TagId id, string name, CategoryId categoryId);
}
