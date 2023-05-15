using Domain.Products;
using Domain.ProductTypes.Tags.TagOptions;
using MediatR;

namespace Application.Products.Commands.RemoveTagOption;

public sealed record RemoveTagOptionFromProductCommand(ProductId ProductId, TagOptionId TagOptionId) : IRequest;
