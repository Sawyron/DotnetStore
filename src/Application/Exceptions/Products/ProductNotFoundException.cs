using Domain.Products;

namespace Application.Exceptions.Products;

internal class ProductNotFoundException : ResourceNotFoundException
{
    public ProductNotFoundException(ProductId productId) :
        base($"Product with id '{productId.Value}' is not found")
    { }
}
