using Application.Core;
using Application.Exceptions.Products;
using Application.TagOptions;
using Domain.ProductTypes.Details;
using Domain.ProductTypes.Tags.TagOptions;
using MediatR;

namespace Application.Products.Commands.RemoveTagOption;

internal class RemoveTagOptionFromProductCommandHandler : IRequestHandler<RemoveTagOptionFromProductCommand>
{
    private readonly IProductRepository _productRepository;
    private readonly ITagOptionRepository _optionRepository;
    private readonly ITagOptionMapper<(TagOptionId Id, string Value, TagId TagId)> _optionMapper;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveTagOptionFromProductCommandHandler(
        IProductRepository productRepository,
        ITagOptionRepository optionRepository,
        ITagOptionMapper<(TagOptionId Id, string Value, TagId TagId)> optionMapper,
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _optionRepository = optionRepository;
        _optionMapper = optionMapper;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(RemoveTagOptionFromProductCommand request, CancellationToken cancellationToken)
    {
        if (await _productRepository.FindByIdAsync(request.ProductId) is null)
        {
            throw new ProductNotFoundException(request.ProductId);
        }
        var configuration = (await _optionRepository.GetProductConfigurationAsync(request.ProductId))
            .Select(tuple => tuple.TagOption.Map(_optionMapper).Id)
            .ToList();
        configuration.Remove(request.TagOptionId);
        await _productRepository.UpdateConfigurationAsync(request.ProductId, configuration);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
