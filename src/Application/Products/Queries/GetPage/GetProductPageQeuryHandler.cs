using Domain.Files;
using Domain.Products;
using MediatR;

namespace Application.Products.Queries.GetPage;

internal class GetProductPageQeuryHandler : IRequestHandler<GetProductPageByCategoryIdQuery, IEnumerable<ProductItemResponse>>
{
    private readonly IProductRepository _productRepository;
    private readonly IProductMapper<ProductItemResponse> _productMapper;
    private readonly IProductMapper<FileId> _productFileIdMapper;

    public GetProductPageQeuryHandler(
        IProductRepository productRepository,
        IProductMapper<ProductItemResponse> productMapper,
        IProductMapper<FileId> productFileIdMapper)
    {
        _productRepository = productRepository;
        _productMapper = productMapper;
        _productFileIdMapper = productFileIdMapper;
    }

    public async Task<IEnumerable<ProductItemResponse>> Handle(GetProductPageByCategoryIdQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<Product> products = await _productRepository.GetCategoryProductsPageAsync(
            request.Offset,
            request.PageSize,
            request.CategoryId);
        return products.Select(p => (Item: p.Map(_productMapper), FileId: p.Map(_productFileIdMapper)))
            .Select(tuple => tuple.Item with { ImageUrl = request.LinkFactory(tuple.FileId) })
            .ToList();
    }
}
