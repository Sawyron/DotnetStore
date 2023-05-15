using Domain.Products;
using MediatR;

namespace Application.Products.Commands.DeleteProduct;

public record DeleteProductCommand(ProductId Id) : IRequest;
