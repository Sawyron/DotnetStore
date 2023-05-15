using Application.Core;
using Application.Exceptions.Products;
using Application.Products;
using Application.Products.Commands.RemoveTagOption;
using Application.TagOptions;
using Domain.Categories;
using Domain.Files;
using Domain.Products;
using Domain.ProductTypes.Details;
using Domain.ProductTypes.Tags.TagOptions;
using Moq;

namespace Application.UnitTests.Products.RemoveTagOption;

public class RemoveTagOptionFromProductCommandHandlerTest
{
    private readonly Mock<IProductRepository> _productRepositoryMock = new();
    private readonly Mock<ITagOptionRepository> _optionRepositoryMock = new();
    private readonly TagOptionTupleMapper _optionMapper = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    [Fact]
    public async Task Handle_Should_Work()
    {
        // Arrange
        var productId = new ProductId(new Guid("1CA97A3A-398A-4ABD-B1D6-AC9FC43B77F7"));
        _productRepositoryMock.Setup(repo => repo.FindByIdAsync(productId))
            .ReturnsAsync(new Product(
                productId,
                "name",
                1000,
                "Desc",
                new StoredFile(new FileId(new Guid()), "path"),
                Array.Empty<TagOptionId>(),
                new CategoryId(new Guid())));
        var optionId = new TagOptionId(new Guid("77645258-E5BC-493E-8B80-44CBB7891201"));
        _optionRepositoryMock.Setup(repo => repo.GetProductConfigurationAsync(productId))
            .ReturnsAsync(new List<(Tag Tag, TagOption TagOption)> 
            {
                (new Tag(
                    new TagId(new Guid()),
                    "name",
                    new CategoryId(new Guid())),
                 new TagOption(optionId,
                 "value",
                 new TagId(new Guid())))
            });
        var handeler = new RemoveTagOptionFromProductCommandHandler(
            _productRepositoryMock.Object,
            _optionRepositoryMock.Object,
            _optionMapper,
            _unitOfWorkMock.Object);
        // Act
        await handeler.Handle(new RemoveTagOptionFromProductCommand(productId, optionId), default);
        // Assert
        _productRepositoryMock.Verify(repo => repo.FindByIdAsync(productId), Times.Once());
        _optionRepositoryMock.Verify(repo => repo.GetProductConfigurationAsync(productId), Times.Once());
        _productRepositoryMock.Verify(repo => repo.UpdateConfigurationAsync(
                productId,
                It.Is<IEnumerable<TagOptionId>>(ids => !ids.Contains(optionId))),
            Times.Once());
        _unitOfWorkMock.Verify(unit => unit.SaveChangesAsync(default), Times.Once());
    }

    [Fact]
    public async Task Hanlde_Should_Throw_ProductNotFoundException_If_ProductDoesNotExsist()
    {
        // Arrange
        var productId = new ProductId(new Guid("1CA97A3A-398A-4ABD-B1D6-AC9FC43B77F7"));
        var handeler = new RemoveTagOptionFromProductCommandHandler(
            _productRepositoryMock.Object,
            _optionRepositoryMock.Object,
            _optionMapper,
            _unitOfWorkMock.Object);
        // Act & Assert
        await Assert.ThrowsAsync<ProductNotFoundException>(async () =>
        {
            await handeler.Handle(new RemoveTagOptionFromProductCommand(productId, new TagOptionId(new Guid())), default);
        });
        _productRepositoryMock.Verify(repo => repo.FindByIdAsync(productId), Times.Once());
        _productRepositoryMock.VerifyNoOtherCalls();
        _optionRepositoryMock.VerifyNoOtherCalls();
        _unitOfWorkMock.VerifyNoOtherCalls();
    }
}
