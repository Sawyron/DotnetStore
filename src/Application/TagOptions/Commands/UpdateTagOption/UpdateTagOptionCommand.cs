using Domain.ProductTypes.Tags.TagOptions;
using Domain.Tags;
using MediatR;

namespace Application.TagOptions.Commands.UpdateTagOption;

public sealed record UpdateTagOptionCommand(TagOptionId Id, string Value, TagId TagId) : IRequest;
