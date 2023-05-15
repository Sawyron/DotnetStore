using Application.Core;
using Application.Exceptions.Products;
using Application.Products;
using Application.Products.Commands.UpdateProduct;
using Domain.Categories;
using Domain.Files;
using Domain.Products;
using Domain.ProductTypes.Tags.TagOptions;
using Moq;

namespace Application.UnitTests.Products.Commands.UpdateProduct;

public class UpdateProductCommandHandlerTest
{
    private readonly Mock<IProductRepository> _productRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    [Fact]
    public async Task Handle_Should_Work()
    {
        // Arrange
        var productId = new ProductId(new Guid("857531EF-7F22-4CE1-A2F9-AD1D18C1046E"));
        _productRepositoryMock.Setup(repo => repo.FindByIdAsync(productId))
            .ReturnsAsync(new Product(
                productId,
                "name",
                100,
                "desc",
                new StoredFile(new FileId(new Guid()), "path"),
                Array.Empty<TagOptionId>(),
                new CategoryId(new Guid())));
        var handler = new UpdateProductCommandHandler(
            _productRepositoryMock.Object,
            _unitOfWorkMock.Object);
        // Act
        await handler.Handle(new UpdateProductCommand(
            productId,
            "new_name",
            1000,
            "new_desc"), default);
        // Assert
        _productRepositoryMock.Verify(repo => repo.FindByIdAsync(productId), Times.Once());
        _productRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Product>()), Times.Once());
        _unitOfWorkMock.Verify(repo => repo.SaveChangesAsync(default), Times.Once());
    }

    [Fact]
    public async Task Handle_Should_Throw_ProductNotFoundException_If_ProductDoesNotExit()
    {
        // Arrange
        var productId = new ProductId(new Guid("857531EF-7F22-4CE1-A2F9-AD1D18C1046E"));
        var handler = new UpdateProductCommandHandler(
            _productRepositoryMock.Object,
            _unitOfWorkMock.Object);
        // Act & Assert
        await Assert.ThrowsAsync<ProductNotFoundException>(async () =>
        {
            await handler.Handle(new UpdateProductCommand(
            productId,
            "new_name",
            1000,
            "new_desc"), default);
        });
        _productRepositoryMock.Verify(repo => repo.FindByIdAsync(productId), Times.Once());
        _productRepositoryMock.VerifyNoOtherCalls();
        _unitOfWorkMock.VerifyNoOtherCalls();
    }
}
