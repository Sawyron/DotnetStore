using Application.Core;
using Domain.Tags;
using Microsoft.AspNetCore.Http;

namespace Application.Exceptions.Tags;

internal class TagNotFoundException : ServerApplicationException
{
    public TagNotFoundException(TagId tagId, int statusCode = StatusCodes.Status404NotFound)
        : base($"Tag with id '{tagId.Value}' is not found", statusCode) { }
}
