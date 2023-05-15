using Application.Core;
using Microsoft.AspNetCore.Http;

namespace Application.Exceptions.Categories;

public sealed class CategoryNameDuplicatedException : ServerApplicationException
{
    public CategoryNameDuplicatedException(string categoryName) :
        base($"Category name '{categoryName}' already exists", StatusCodes.Status409Conflict)
    { }
}
