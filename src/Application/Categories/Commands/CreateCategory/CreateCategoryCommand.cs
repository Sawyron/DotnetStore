using Domain.Categories;
using MediatR;

namespace Application.Categories.Commands.CreateCategory;

public sealed record CreateCategoryCommand(string Name, CategoryId? ParentId) : IRequest<CategoryId>;
