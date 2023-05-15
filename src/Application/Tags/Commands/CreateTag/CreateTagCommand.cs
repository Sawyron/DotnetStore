using Domain.Categories;
using Domain.ProductTypes.Details;
using MediatR;

namespace Application.Tags.Commands.CreateTag;

public record CreateTagCommand(string Name, CategoryId CategoryId) : IRequest<TagId>;