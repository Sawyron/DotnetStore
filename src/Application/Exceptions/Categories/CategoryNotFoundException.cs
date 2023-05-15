using Application.Core;
using Domain.Categories;
using Microsoft.AspNetCore.Http;

namespace Application.Exceptions.Categories;

public sealed class CategoryNotFoundException : ServerApplicationException
{

    public CategoryNotFoundException(CategoryId categoryId, int statusCode = StatusCodes.Status404NotFound) :
        base($"Category with id '{categoryId.Value}' is not found", statusCode)
    { }

}
