using Domain.ProductTypes.Tags.TagOptions;
using MediatR;

namespace Application.TagOptions.Commands.DeleteTagOption;

public record DeleteTagOptionCommand(TagOptionId Id) : IRequest;
