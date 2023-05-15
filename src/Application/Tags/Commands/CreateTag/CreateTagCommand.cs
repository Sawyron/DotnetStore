using Domain.Categories;
using Domain.Tags;
using MediatR;

namespace Application.Tags.Commands.CreateTag;

public record CreateTagCommand(string Name, CategoryId CategoryId) : IRequest<TagId>;