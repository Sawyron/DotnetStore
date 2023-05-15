using Application.Exceptions.Products;
using Application.TagOptions;
using Domain.Categories;
using Domain.Files;
using Domain.Products;
using Domain.ProductTypes.Details;
using Domain.ProductTypes.Tags.TagOptions;
using MediatR;

namespace Application.Products.Queries.GetById;

internal class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductResponse>
{
    private readonly IProductRepository _productRepository;
    private readonly ITagOptionRepository _tagOptionRepository;
    private readonly IProductMapper<ProductResponse> _productMapper;
    private readonly ITagMapper<(TagId TagId, string Name, CategoryId CategoryId)> _tagMapper;
    private readonly ITagOptionMapper<(TagOptionId TagOptionId, string Value, TagId TagId)> _tagOptionMapper;
    private readonly IProductMapper<FileId> _fileIdMapper;

    public GetProductByIdQueryHandler(
        IProductRepository productRepository,
        ITagOptionRepository tagOptionRepository,
        IProductMapper<ProductResponse> productMapper,
        ITagMapper<(TagId TagId, string Name, CategoryId CategoryId)> tagMapper,
        ITagOptionMapper<(TagOptionId TagOptionId, string Value, TagId TagId)> tagOptionMapper,
        IProductMapper<FileId> fileIdMapper)
    {
        _productRepository = productRepository;
        _tagOptionRepository = tagOptionRepository;
        _productMapper = productMapper;
        _tagMapper = tagMapper;
        _tagOptionMapper = tagOptionMapper;
        _fileIdMapper = fileIdMapper;
    }

    public async Task<ProductResponse> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        Product product = await _productRepository.FindByIdAsync(request.Id)
            ?? throw new ProductNotFoundException(request.Id);
        IEnumerable<(Tag Tag, TagOption TagOption)> options = await _tagOptionRepository.GetProductConfigurationAsync(request.Id);
        ProductResponse response = product.Map(_productMapper);
        List<ProductTagOptionResponse> configuration = options.Select(tuple =>
        {
            (TagId TagId, string Name, CategoryId CategoryId) tagTuple = tuple.Tag.Map(_tagMapper);
            (TagOptionId TagOptionId, string Value, TagId TagId) optionTuple = tuple.TagOption.Map(_tagOptionMapper);
            return new ProductTagOptionResponse(optionTuple.TagOptionId.Value, tagTuple.Name, optionTuple.Value);
        }).ToList();
        return response with { Configuration = configuration, ImageUrl = request.LinkFactory(product.Map(_fileIdMapper)) };
    }
}
