using Application.Core;
using Domain.Categories;
using Microsoft.AspNetCore.Http;

namespace Application.Exceptions.Tags;

internal class TagNameConflictException : ServerApplicationException
{
    public TagNameConflictException(string name, CategoryId categoryId)
        : base($"Tag with name '{name}' already exists in category with id '{categoryId.Value}'", StatusCodes.Status409Conflict)
    { }
}
