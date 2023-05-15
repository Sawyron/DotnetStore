using Domain.Categories;
using Domain.ProductTypes.Details;
using MediatR;

namespace Application.Tags.Commands.UpdateTag;

public sealed record UpdateTagCommand(TagId Id, string Name, CategoryId CategoryId) : IRequest;
