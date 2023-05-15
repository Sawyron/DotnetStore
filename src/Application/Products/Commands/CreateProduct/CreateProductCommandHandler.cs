using Application.Core;
using Application.Exceptions.Products;
using Application.Files;
using Application.TagOptions;
using Domain.Files;
using Domain.Products;
using Domain.ProductTypes.Details;
using Domain.ProductTypes.Tags.TagOptions;
using MediatR;

namespace Application.Products.Commands.CreateProduct;

internal class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductId>
{
    private readonly IProductRepository _productRepository;
    private readonly ITagOptionRepository _tagOptionRepository;
    private readonly ITagOptionMapper<(TagOptionId TagOptionId, string Value, TagId TagId)> _tagOptionMapper;
    private readonly IFileStorage _fileStorage;
    private readonly IUnitOfWork _unitOfWork;

    public CreateProductCommandHandler(
        IProductRepository productRepository,
        ITagOptionRepository tagOptionRepository,
        ITagOptionMapper<(TagOptionId TagOptionId, string Value, TagId TagId)> tagOptionMapper,
        IFileStorage fileStorage,
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _tagOptionRepository = tagOptionRepository;
        _tagOptionMapper = tagOptionMapper;
        _fileStorage = fileStorage;
        _unitOfWork = unitOfWork;
    }

    public async Task<ProductId> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        IDictionary<TagId, List<TagOption>> tagIdToOptionDict = await _tagOptionRepository.GroupByTagIdAsync(request.TagOptionIds);
        if (tagIdToOptionDict.Values.Any(options => options.Count > 1))
        {
            throw new TagOptionIdsTagCollisionException();
        }
        var correctOptionIds = (await _tagOptionRepository.FilterCategoryTagOptions(request.TagOptionIds, request.CategoryId))
            .Select(o => o.Map(_tagOptionMapper).TagOptionId)
            .ToList();
        var wrongTagOptionIds = request.TagOptionIds.Where(id => !correctOptionIds.Contains(id)).ToList();
        if (wrongTagOptionIds.Any())
        {
            throw new TagOptionIdsCategoryConflictException(wrongTagOptionIds, request.CategoryId);
        }
        StoredFile photo = await _fileStorage.SaveFileAsync(request.Photo.Content, request.Photo.Extension);
        var productId = new ProductId(Guid.NewGuid());
        var product = new Product(
            productId,
            request.Name,
            request.Price,
            request.Description,
            photo,
            request.TagOptionIds,
            request.CategoryId);
        await _productRepository.AddAsync(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return productId;
    }
}
