using Application.Core;
using Application.Exceptions.Products;
using Application.Exceptions.TagOptions;
using Application.TagOptions;
using Domain.Categories;
using Domain.Products;
using Domain.ProductTypes.Details;
using Domain.ProductTypes.Tags.TagOptions;
using MediatR;

namespace Application.Products.Commands.AddTagOption;

internal class AddTagOptionToProductCommandHandler : IRequestHandler<AddTagOptionToProductCommand>
{
    private readonly ITagOptionRepository _optionRepository;
    private readonly IProductRepository _productRepository;
    private readonly IProductMapper<CategoryId> _productCategoryIdMapper;
    private readonly ITagMapper<(TagId Id, string Name, CategoryId CategoryId)> _tagMapper;
    private readonly ITagOptionMapper<(TagOptionId Id, string Value, TagId TagId)> _optionMapper;
    private readonly IUnitOfWork _unitOfWork;

    public AddTagOptionToProductCommandHandler(
        ITagOptionRepository optionRepository,
        IProductRepository productRepository,
        IProductMapper<CategoryId> productCategoryIdMapper,
        ITagMapper<(TagId Id, string Name, CategoryId CategoryId)> tagMapper,
        ITagOptionMapper<(TagOptionId Id, string Value, TagId TagId)> optionMapper,
        IUnitOfWork unitOfWork)
    {
        _optionRepository = optionRepository;
        _productRepository = productRepository;
        _productCategoryIdMapper = productCategoryIdMapper;
        _tagMapper = tagMapper;
        _optionMapper = optionMapper;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(AddTagOptionToProductCommand request, CancellationToken cancellationToken)
    {
        Product product = await _productRepository.FindByIdAsync(request.ProductId)
            ?? throw new ProductNotFoundException(request.ProductId);
        TagOption option = await _optionRepository.FindByIdAsync(request.TagOptionId)
            ?? throw new TagOptionNotFoundException(request.TagOptionId);
        CategoryId productCategoryId = product.Map(_productCategoryIdMapper);
        if (!await _optionRepository.BelongsToCategory(request.TagOptionId, productCategoryId))
        {
            throw new TagOptionIdsCategoryConflictException(new TagOptionId[] { request.TagOptionId }, productCategoryId);
        }
        IEnumerable<(Tag Tag, TagOption TagOption)> configuration = await _optionRepository.GetProductConfigurationAsync(request.ProductId);
        List<TagId> tagIds = configuration.Select(tuple => tuple.Tag.Map(_tagMapper).Id).ToList();
        (TagOptionId Id, string Value, TagId TagId) optionTuple = option.Map(_optionMapper);
        if (tagIds.Contains(optionTuple.TagId))
        {
            throw new TagOptionIdsTagCollisionException();
        }
        List<TagOptionId> optionIds = configuration.Select(tuple => tuple.TagOption.Map(_optionMapper).Id).ToList();
        optionIds.Add(request.TagOptionId);
        await _productRepository.UpdateConfigurationAsync(request.ProductId, optionIds);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
