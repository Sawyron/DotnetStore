using Domain.Tags;
using MediatR;

namespace Application.Tags.Commands.DeleteTag;

public record DeleteTagCommand(TagId Id) : IRequest;
