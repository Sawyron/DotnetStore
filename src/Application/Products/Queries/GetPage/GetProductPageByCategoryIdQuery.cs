using Application.Core;
using Domain.Categories;
using Domain.Files;

namespace Application.Products.Queries.GetPage;

public record GetProductPageByCategoryIdQuery(
    CategoryId CategoryId,
    int Offset,
    int PageSize,
    Func<FileId, string> LinkFactory) : IGetPageQuery<ProductItemResponse>;
