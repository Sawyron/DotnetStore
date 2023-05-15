using Domain.Categories;
using MediatR;

namespace Application.Categories.Queries.GetCategoryById;

public sealed record GetCategoryByIdQuery(CategoryId Id) : IRequest<CategoryResponse>;
