using Domain.Categories;
using MediatR;

namespace Application.Categories.Commands.DeleteCategory;

public sealed record DeleteCategoryCommand(CategoryId Id) : IRequest;
