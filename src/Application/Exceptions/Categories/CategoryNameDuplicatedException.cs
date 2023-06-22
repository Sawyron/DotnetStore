namespace Application.Exceptions.Categories;

public sealed class CategoryNameDuplicatedException : ResourceDublicationException
{
    public CategoryNameDuplicatedException(string categoryName) :
        base($"Category name '{categoryName}' already exists")
    { }
}
