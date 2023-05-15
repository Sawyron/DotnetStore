using Application.Core;
using Microsoft.AspNetCore.Http;

namespace Application.Exceptions.Products;

internal class TagOptionIdsTagCollisionException : ServerApplicationException
{
    public TagOptionIdsTagCollisionException()
        : base("More than one tag options ids belong to the same tag", StatusCodes.Status409Conflict) { }
}
