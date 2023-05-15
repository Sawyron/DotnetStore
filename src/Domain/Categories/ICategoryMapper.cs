namespace Domain.Categories;

public interface ICategoryMapper<T>
{
    T Map(CategoryId id, string name);
}
