using Domain.Categories;
using MediatR;

namespace Application.Categories.Commands.UpdateCategory;

public sealed record UpdateCategoryCommand(CategoryId Id, string Name, CategoryId? ParentId) : IRequest;
