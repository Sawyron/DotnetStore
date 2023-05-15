using Application.Core;
using Application.Exceptions.Products;
using Application.Files;
using Application.Images;
using Domain.Files;
using Domain.Products;
using MediatR;

namespace Application.Products.Commands.ReplacePhoto;

internal class ReplaceProductPhotoCommandHandler : IRequestHandler<ReplaceProductPhotoCommand>
{
    private readonly IProductRepository _productRepository;
    private readonly IFileRepository _fileRepository;
    private readonly IFileStorage _fileStorage;
    private readonly IProductMapper<StoredFile> _productMapper;
    private readonly IFileMapper<FileId> _fileIdMapper;
    private readonly IUnitOfWork _unitOfWork;

    public ReplaceProductPhotoCommandHandler(
        IProductRepository productRepository,
        IFileRepository fileRepository,
        IFileStorage fileStorage,
        IProductMapper<StoredFile> productMapper,
        IFileMapper<FileId> fileIdMapper,
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _fileRepository = fileRepository;
        _fileStorage = fileStorage;
        _productMapper = productMapper;
        _fileIdMapper = fileIdMapper;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(ReplaceProductPhotoCommand request, CancellationToken cancellationToken)
    {
        Product product = await _productRepository.FindByIdAsync(request.ProductId)
            ?? throw new ProductNotFoundException(request.ProductId);
        StoredFile existingPhoto = product.Map(_productMapper);
        await _fileRepository.DeleteAsync(existingPhoto.Map(_fileIdMapper));
        StoredFile updatedFile = await _fileStorage.SaveFileAsync(request.Photo.Content, request.Photo.Extension);
        _fileStorage.DeleteFile(existingPhoto);
        await _fileRepository.AddAsync(updatedFile);
        await _productRepository.UpdatePhoto(request.ProductId, updatedFile.Map(_fileIdMapper));
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
