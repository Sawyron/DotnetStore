using Application.Core;
using Application.Exceptions.Products;
using Application.Files;
using Domain.Files;
using Domain.Products;
using MediatR;

namespace Application.Products.Commands.DeleteProduct;

internal class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand>
{
    private readonly IProductRepository _productRepository;
    private readonly IFileStorage _fileStorage;
    private readonly IProductMapper<StoredFile> _productFileMapper;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteProductCommandHandler(
        IProductRepository productRepository,
        IFileStorage fileStorage,
        IProductMapper<StoredFile> productFileMapper,
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _fileStorage = fileStorage;
        _productFileMapper = productFileMapper;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        Product product = await _productRepository.FindByIdAsync(request.Id)
            ?? throw new ProductNotFoundException(request.Id);
        StoredFile image = product.Map(_productFileMapper);
        await _productRepository.DeleteAsync(request.Id);
        _fileStorage.DeleteFile(image);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
