using Domain.ProductTypes.Details;
using Domain.ProductTypes.Tags.TagOptions;
using MediatR;

namespace Application.TagOptions.Commands.CreateTagOption;

public record CreateTagOptionCommand(string Value, TagId TagId) : IRequest<TagOptionId>;
