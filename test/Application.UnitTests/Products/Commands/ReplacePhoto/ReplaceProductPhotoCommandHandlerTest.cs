using Application.Core;
using Application.Exceptions.Products;
using Application.Files;
using Application.Files.Commands.CreateFile;
using Application.Images;
using Application.Products;
using Application.Products.Commands.DeleteProduct;
using Application.Products.Commands.ReplacePhoto;
using Domain.Categories;
using Domain.Files;
using Domain.Products;
using Domain.ProductTypes.Tags.TagOptions;
using Moq;

namespace Application.UnitTests.Products.Commands.ReplacePhoto;

public class ReplaceProductPhotoCommandHandlerTest
{
    private readonly Mock<IProductRepository> _productRepositoryMock = new();
    private readonly Mock<IFileRepository> _fileRepositoryMock = new();
    private readonly Mock<IFileStorage> _fileStorageMock = new();
    private readonly ProductFileMapper _productFileMapper = new();
    private readonly FileIdMapper _fileIdMapper = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();


    [Fact]
    public async Task Handle_Should_Work()
    {
        // Arrange
        var productId = new ProductId(new Guid("620BDCF0-893E-4B30-9E96-7737B04AF646"));
        var exsistingFileId = new FileId(new Guid("35646A64-1687-4DC3-9695-9C4444D5168D"));
        var existingPhoto = new StoredFile(exsistingFileId, "path");
        _productRepositoryMock.Setup(repo => repo.FindByIdAsync(productId))
            .ReturnsAsync(new Product(
                productId,
                "name",
                100,
                "desc",
                existingPhoto,
                Array.Empty<TagOptionId>(),
                new CategoryId(new Guid())));
        var updatedFileId = new FileId(new Guid("07816A72-1449-4413-B90E-943EB0C8CB61"));
        string extension = ".png";
        byte[] content = new byte[] { 1, 2 };
        var updatedFile = new StoredFile(updatedFileId, "new_path");
        _fileStorageMock.Setup(storage => storage.SaveFileAsync(content, extension))
            .ReturnsAsync(updatedFile);
        var handler = new ReplaceProductPhotoCommandHandler(
            _productRepositoryMock.Object,
            _fileRepositoryMock.Object,
            _fileStorageMock.Object,
            _productFileMapper,
            _fileIdMapper,
            _unitOfWorkMock.Object);
        // Act
        await handler.Handle(new ReplaceProductPhotoCommand(
            productId,
            new CreateFileCommand(content, extension)), default);
        // Assert
        _productRepositoryMock.Verify(repo => repo.FindByIdAsync(productId), Times.Once());
        _fileStorageMock.Verify(storage => storage.DeleteFile(existingPhoto), Times.Once());
        _fileStorageMock.Verify(storage => storage.SaveFileAsync(content, extension), Times.Once());
        _fileRepositoryMock.Verify(repo => repo.DeleteAsync(exsistingFileId), Times.Once());
        _productRepositoryMock.Verify(repo => repo.UpdatePhoto(productId, updatedFileId));
        _fileRepositoryMock.Verify(repo => repo.AddAsync(updatedFile), Times.Once());
        _unitOfWorkMock.Verify(unit => unit.SaveChangesAsync(default));
    }

    [Fact]
    public async Task Handle_Should_Throw_ProductNotFound_Exception_If_ProductDoesNotExist()
    {
        // Arrange
        var productId = new ProductId(new Guid("620BDCF0-893E-4B30-9E96-7737B04AF646"));
        var handler = new ReplaceProductPhotoCommandHandler(
            _productRepositoryMock.Object,
            _fileRepositoryMock.Object,
            _fileStorageMock.Object,
            _productFileMapper,
            _fileIdMapper,
            _unitOfWorkMock.Object);
        // Act & Assert
        await Assert.ThrowsAsync<ProductNotFoundException>(async () =>
        {
            await handler.Handle(new ReplaceProductPhotoCommand(
                productId,
                new CreateFileCommand(new byte[] { 1, 2 }, ".ext")), default);
        });
        _productRepositoryMock.Verify(repo => repo.FindByIdAsync(productId), Times.Once());
        _productRepositoryMock.VerifyNoOtherCalls();
        _fileRepositoryMock.VerifyNoOtherCalls();
        _fileStorageMock.VerifyNoOtherCalls();
        _unitOfWorkMock.VerifyNoOtherCalls();
    }
}
