using Application.Core;
using Application.Exceptions.Products;
using Application.Files;
using Application.Images;
using Domain.Files;
using Domain.Products;
using MediatR;

namespace Application.Products.Commands.DeleteProduct;

internal class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand>
{
    private readonly IProductRepository _productRepository;
    private readonly IFileRepository _fileRepository;
    private readonly IFileStorage _fileStorage;
    private readonly IFileMapper<FileId> _fileIdMapper;
    private readonly IProductMapper<StoredFile> _productFileMapper;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteProductCommandHandler(
        IProductRepository productRepository,
        IFileRepository fileRepository,
        IFileStorage fileStorage,
        IFileMapper<FileId> fileIdMapper,
        IProductMapper<StoredFile> productFileMapper,
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _fileRepository = fileRepository;
        _fileStorage = fileStorage;
        _fileIdMapper = fileIdMapper;
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
        await _fileRepository.DeleteAsync(image.Map(_fileIdMapper));
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
