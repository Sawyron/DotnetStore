using Application.Core;
using Domain.ProductTypes.Tags.TagOptions;
using Microsoft.AspNetCore.Http;

namespace Application.Exceptions.TagOptions;

internal class TagOptionNotFoundException : ServerApplicationException
{
    public TagOptionNotFoundException(TagOptionId id)
        : base($"Tag optins with id '{id.Value}' is not found", StatusCodes.Status404NotFound) { }
}
