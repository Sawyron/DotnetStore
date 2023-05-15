using Domain.Files;
using Domain.Products;
using MediatR;

namespace Application.Products.Queries.GetById;

public record GetProductByIdQuery(ProductId Id, Func<FileId, string> LinkFactory) : IRequest<ProductResponse>;
