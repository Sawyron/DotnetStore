using Application.Core;
using Application.Exceptions.Products;
using Application.Files;
using Application.Products;
using Application.Products.Commands.DeleteProduct;
using Domain.Categories;
using Domain.Files;
using Domain.Products;
using Domain.ProductTypes.Tags.TagOptions;
using Moq;

namespace Application.UnitTests.Products.Commands.DeleteProduct;

public class DeleteProductCommandHandlerTest
{
    private readonly Mock<IProductRepository> _repositoryMock;
    private readonly Mock<IFileStorage> _storageMock;
    private readonly ProductFileMapper _mapper;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public DeleteProductCommandHandlerTest()
    {
        _repositoryMock = new();
        _storageMock = new();
        _mapper = new();
        _unitOfWorkMock = new();
    }

    [Fact]
    public async Task Hanlde_Should_Work()
    {
        // Arrange
        var productId = new ProductId(new Guid("6307B68D-B15E-4394-9F2C-868EB45C121A"));
        var file = new StoredFile(new FileId(new Guid("147C685F-E2C8-40F4-A830-92D3AF41CB98")), string.Empty);
        _repositoryMock.Setup(repo => repo.FindByIdAsync(productId))
            .ReturnsAsync(new Product(
                productId,
                string.Empty,
                100,
                string.Empty,
                file,
                Array.Empty<TagOptionId>(),
                new CategoryId(new Guid("D0703F79-3D9E-4188-935E-0B72E6658AB5"))));
        var handler = new DeleteProductCommandHandler(
            _repositoryMock.Object,
            _storageMock.Object,
            _mapper,
            _unitOfWorkMock.Object);
        // Act
        await handler.Handle(new DeleteProductCommand(productId), default);
        // Assert
        _repositoryMock.Verify(repo => repo.FindByIdAsync(productId), Times.Once());
        _repositoryMock.Verify(repo => repo.DeleteAsync(productId), Times.Once());
        _storageMock.Verify(s => s.DeleteFile(file), Times.Once());
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(default));
    }

    [Fact]
    public async Task Hanlde_Should_Throw_ProductNotFoundException_If_ProductDoesNotExist()
    {
        // Arrange
        var productId = new ProductId(new Guid("6307B68D-B15E-4394-9F2C-868EB45C121A"));
        _repositoryMock.Setup(repo => repo.FindByIdAsync(productId))
            .ReturnsAsync((Product)null!);
        var handler = new DeleteProductCommandHandler(
            _repositoryMock.Object,
            _storageMock.Object,
            _mapper,
            _unitOfWorkMock.Object);
        // Act & Assert
        await Assert.ThrowsAsync<ProductNotFoundException>(async () =>
        {
            await handler.Handle(new DeleteProductCommand(productId), default);
        });
        _repositoryMock.Verify(repo => repo.FindByIdAsync(productId), Times.Once());
        _repositoryMock.VerifyNoOtherCalls();
        _storageMock.VerifyNoOtherCalls();
        _unitOfWorkMock.VerifyNoOtherCalls();
    }
}
