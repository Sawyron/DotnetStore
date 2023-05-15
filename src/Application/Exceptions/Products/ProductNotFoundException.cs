using Application.Core;
using Domain.Products;
using Microsoft.AspNetCore.Http;

namespace Application.Exceptions.Products;

internal class ProductNotFoundException : ServerApplicationException
{
    public ProductNotFoundException(ProductId productId, int statusCode = StatusCodes.Status404NotFound) :
        base($"Product with id '{productId.Value}' is not found", statusCode)
    { }
}
