using Application.Tags.Commands.CreateTag;
using Domain.Categories;
using MediatR;

namespace Application.Tags.Commands.CreateTags;

public record CreateTagsCommand(
    CategoryId CategoryId,
    IEnumerable<CreateTagCommand> Tags) : IRequest<IEnumerable<TagResponse>>;
