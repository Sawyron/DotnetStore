using Application.Tags.Commands.CreateTags;
using Domain.Categories;
using MediatR;

namespace Application.Tags.Queries.GetCategoryTags;

public sealed record GetCategoryTagsQuery(CategoryId CategoryId) : IRequest<IEnumerable<TagResponse>>;
