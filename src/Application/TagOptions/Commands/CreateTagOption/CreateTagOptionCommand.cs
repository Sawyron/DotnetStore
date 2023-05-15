using Domain.ProductTypes.Tags.TagOptions;
using Domain.Tags;
using MediatR;

namespace Application.TagOptions.Commands.CreateTagOption;

public record CreateTagOptionCommand(string Value, TagId TagId) : IRequest<TagOptionId>;
