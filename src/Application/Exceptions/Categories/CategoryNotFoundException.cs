using Domain.Categories;

namespace Application.Exceptions.Categories;

public sealed class CategoryNotFoundException : ResourceNotFoundException
{

    public CategoryNotFoundException(CategoryId categoryId) :
        base($"Category with id '{categoryId.Value}' is not found")
    { }

}
