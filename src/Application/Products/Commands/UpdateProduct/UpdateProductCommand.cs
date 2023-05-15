using Domain.Products;
using MediatR;

namespace Application.Products.Commands.UpdateProduct;

public sealed record UpdateProductCommand(
    ProductId Id,
    string Name,
    int Price,
    string Description) : IRequest;
