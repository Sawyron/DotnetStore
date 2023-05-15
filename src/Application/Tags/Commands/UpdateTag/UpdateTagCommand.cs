using Domain.Categories;
using Domain.Tags;
using MediatR;

namespace Application.Tags.Commands.UpdateTag;

public sealed record UpdateTagCommand(TagId Id, string Name, CategoryId CategoryId) : IRequest;
