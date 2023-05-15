using Domain.Products;
using Domain.ProductTypes.Tags.TagOptions;
using MediatR;

namespace Application.Products.Commands.AddTagOption;

public sealed record AddTagOptionToProductCommand(ProductId ProductId, TagOptionId TagOptionId) : IRequest;
