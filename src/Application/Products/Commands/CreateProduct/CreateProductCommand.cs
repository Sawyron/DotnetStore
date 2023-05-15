using Application.Files.Commands.CreateFile;
using Domain.Categories;
using Domain.Products;
using Domain.ProductTypes.Tags.TagOptions;
using MediatR;

namespace Application.Products.Commands.CreateProduct;

public record CreateProductCommand(
    string Name,
    int Price,
    string Description,
    IEnumerable<TagOptionId> TagOptionIds,
    CreateFileCommand Photo,
    CategoryId CategoryId) : IRequest<ProductId>;
