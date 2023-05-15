using Application.Core;
using Application.Exceptions.Products;
using Application.Exceptions.TagOptions;
using Application.Products;
using Application.Products.Commands.AddTagOption;
using Application.TagOptions;
using Application.Tags;
using Domain.Categories;
using Domain.Files;
using Domain.Products;
using Domain.ProductTypes.Tags.TagOptions;
using Domain.Tags;
using Moq;

namespace Application.UnitTests.Products.Commands.AddTagOption;

public class AddTagOptionToProductCommandHandlerTest
{
    private readonly Mock<ITagOptionRepository> _optionRepositoryMock = new();
    private readonly Mock<IProductRepository> _productRepositoryMock = new();
    private readonly ProductCategoryIdMapper _productCategoryIdMapper = new();
    private readonly TagTupleMapper _tagMapper = new();
    private readonly TagOptionTupleMapper _optionMapper = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();


    [Fact]
    public async Task Handle_Should_Work()
    {
        // Arrange
        var productId = new ProductId(new Guid("28AA2A67-A9DC-408A-ABAB-9EAFDA047066"));
        var categoryId = new CategoryId(new Guid("28AA2A67-A9DC-408A-ABAB-9EAFDA047066"));
        var optionId = new TagOptionId(new Guid("28AA2A67-A9DC-408A-ABAB-9EAFDA047066"));
        _productRepositoryMock.Setup(repo => repo.FindByIdAsync(productId))
            .ReturnsAsync(new Product(
                productId,
                "name",
                1000,
                "Desc",
                new StoredFile(new FileId(new Guid()), "path"),
                Array.Empty<TagOptionId>(),
                categoryId));
        _optionRepositoryMock.Setup(repo => repo.FindByIdAsync(optionId))
            .ReturnsAsync(new TagOption(optionId, "value", new TagId(new Guid())));
        _optionRepositoryMock.Setup(repo => repo.BelongsToCategory(optionId, categoryId))
            .ReturnsAsync(true);
        _optionRepositoryMock.Setup(repo => repo.GetProductConfigurationAsync(productId))
            .ReturnsAsync(Array.Empty<(Tag, TagOption)>());
        var handler = new AddTagOptionToProductCommandHandler(
            _optionRepositoryMock.Object,
            _productRepositoryMock.Object,
            _productCategoryIdMapper,
            _tagMapper,
            _optionMapper,
            _unitOfWorkMock.Object);
        // Act
        await handler.Handle(new AddTagOptionToProductCommand(productId, optionId), default);
        // Assert
        _productRepositoryMock.Verify(repo => repo.FindByIdAsync(productId), Times.Once());
        _optionRepositoryMock.Verify(repo => repo.FindByIdAsync(optionId), Times.Once());
        _optionRepositoryMock.Verify(repo => repo.BelongsToCategory(optionId, categoryId), Times.Once());
        _optionRepositoryMock.Verify(repo => repo.GetProductConfigurationAsync(productId), Times.Once());
        _productRepositoryMock.Verify(repo => repo.UpdateConfigurationAsync(productId, new TagOptionId[] { optionId }), Times.Once());
        _unitOfWorkMock.Verify(repo => repo.SaveChangesAsync(default), Times.Once());
    }

    [Fact]
    public async Task Handle_Should_Throw_ProductNotFoundException_If_ProductDoesNotExist()
    {
        // Arrange
        var productId = new ProductId(new Guid("28AA2A67-A9DC-408A-ABAB-9EAFDA047066"));
        var optionId = new TagOptionId(new Guid("28AA2A67-A9DC-408A-ABAB-9EAFDA047066"));
        var handler = new AddTagOptionToProductCommandHandler(
            _optionRepositoryMock.Object,
            _productRepositoryMock.Object,
            _productCategoryIdMapper,
            _tagMapper,
            _optionMapper,
            _unitOfWorkMock.Object);
        // Act & Assert
        await Assert.ThrowsAsync<ProductNotFoundException>(async () =>
        {
            await handler.Handle(new AddTagOptionToProductCommand(productId, optionId), default);
        });
        _productRepositoryMock.Verify(repo => repo.FindByIdAsync(productId), Times.Once());
        _productRepositoryMock.VerifyNoOtherCalls();
        _optionRepositoryMock.VerifyNoOtherCalls();
        _unitOfWorkMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Handle_Should_Throw_TagOptionNotFoundException_If_OptionDoesNotExist()
    {
        // Arrange
        var productId = new ProductId(new Guid("28AA2A67-A9DC-408A-ABAB-9EAFDA047066"));
        var optionId = new TagOptionId(new Guid("28AA2A67-A9DC-408A-ABAB-9EAFDA047066"));
        var handler = new AddTagOptionToProductCommandHandler(
            _optionRepositoryMock.Object,
            _productRepositoryMock.Object,
            _productCategoryIdMapper,
            _tagMapper,
            _optionMapper,
            _unitOfWorkMock.Object);
        _productRepositoryMock.Setup(repo => repo.FindByIdAsync(productId))
            .ReturnsAsync(new Product(
                productId,
                "name",
                1000,
                "Desc",
                new StoredFile(new FileId(new Guid()), "path"),
                Array.Empty<TagOptionId>(),
                new CategoryId(new Guid())));
        // Act & Assert
        await Assert.ThrowsAsync<TagOptionNotFoundException>(async () =>
        {
            await handler.Handle(new AddTagOptionToProductCommand(productId, optionId), default);
        });
        _productRepositoryMock.Verify(repo => repo.FindByIdAsync(productId), Times.Once());
        _optionRepositoryMock.Verify(repo => repo.FindByIdAsync(optionId), Times.Once());
        _productRepositoryMock.VerifyNoOtherCalls();
        _optionRepositoryMock.VerifyNoOtherCalls();
        _unitOfWorkMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Handle_Should_Throw_TagOptionIdsCategoryConflictException_If_OptindDoesNotBelongToCategory()
    {
        // Arrange
        var productId = new ProductId(new Guid("28AA2A67-A9DC-408A-ABAB-9EAFDA047066"));
        var categoryId = new CategoryId(new Guid("28AA2A67-A9DC-408A-ABAB-9EAFDA047066"));
        var optionId = new TagOptionId(new Guid("28AA2A67-A9DC-408A-ABAB-9EAFDA047066"));
        _productRepositoryMock.Setup(repo => repo.FindByIdAsync(productId))
            .ReturnsAsync(new Product(
                productId,
                "name",
                1000,
                "Desc",
                new StoredFile(new FileId(new Guid()), "path"),
                Array.Empty<TagOptionId>(),
                categoryId));
        _optionRepositoryMock.Setup(repo => repo.FindByIdAsync(optionId))
            .ReturnsAsync(new TagOption(optionId, "value", new TagId(new Guid())));
        _optionRepositoryMock.Setup(repo => repo.BelongsToCategory(optionId, categoryId))
            .ReturnsAsync(false);
        var handler = new AddTagOptionToProductCommandHandler(
            _optionRepositoryMock.Object,
            _productRepositoryMock.Object,
            _productCategoryIdMapper,
            _tagMapper,
            _optionMapper,
            _unitOfWorkMock.Object);
        // Act & Assert
        await Assert.ThrowsAsync<TagOptionIdsCategoryConflictException>(async () =>
        {
            await handler.Handle(new AddTagOptionToProductCommand(productId, optionId), default);
        });
        _productRepositoryMock.Verify(repo => repo.FindByIdAsync(productId), Times.Once());
        _optionRepositoryMock.Verify(repo => repo.FindByIdAsync(optionId), Times.Once());
        _optionRepositoryMock.Verify(repo => repo.BelongsToCategory(optionId, categoryId), Times.Once());
        _productRepositoryMock.VerifyNoOtherCalls();
        _optionRepositoryMock.VerifyNoOtherCalls();
        _unitOfWorkMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Handle_Should_Throw_TagOptionIdsTagCollisionException_If_ProductHaveAnotherTagOption()
    {
        // Arrange
        var productId = new ProductId(new Guid("28AA2A67-A9DC-408A-ABAB-9EAFDA047066"));
        var categoryId = new CategoryId(new Guid("28AA2A67-A9DC-408A-ABAB-9EAFDA047066"));
        var optionId = new TagOptionId(new Guid("28AA2A67-A9DC-408A-ABAB-9EAFDA047066"));
        var tagId = new TagId(new Guid("28AA2A67-A9DC-408A-ABAB-9EAFDA047066"));
        _productRepositoryMock.Setup(repo => repo.FindByIdAsync(productId))
            .ReturnsAsync(new Product(
                productId,
                "name",
                1000,
                "Desc",
                new StoredFile(new FileId(new Guid()), "path"),
                Array.Empty<TagOptionId>(),
                categoryId));
        _optionRepositoryMock.Setup(repo => repo.FindByIdAsync(optionId))
            .ReturnsAsync(new TagOption(optionId, "value", tagId));
        _optionRepositoryMock.Setup(repo => repo.BelongsToCategory(optionId, categoryId))
            .ReturnsAsync(true);
        _optionRepositoryMock.Setup(repo => repo.GetProductConfigurationAsync(productId))
            .ReturnsAsync(new List<(Tag, TagOption)>
            {
                (new Tag(
                    tagId,
                    "name", categoryId),
                new TagOption(optionId,
                    "value",
                    tagId))
            });
        var handler = new AddTagOptionToProductCommandHandler(
            _optionRepositoryMock.Object,
            _productRepositoryMock.Object,
            _productCategoryIdMapper,
            _tagMapper,
            _optionMapper,
            _unitOfWorkMock.Object);
        // Act
        await Assert.ThrowsAsync<TagOptionIdsTagCollisionException>(async () =>
        {
            await handler.Handle(new AddTagOptionToProductCommand(productId, optionId), default);
        });
        // Assert
        _productRepositoryMock.Verify(repo => repo.FindByIdAsync(productId), Times.Once());
        _optionRepositoryMock.Verify(repo => repo.FindByIdAsync(optionId), Times.Once());
        _optionRepositoryMock.Verify(repo => repo.BelongsToCategory(optionId, categoryId), Times.Once());
        _optionRepositoryMock.Verify(repo => repo.GetProductConfigurationAsync(productId), Times.Once());
        _productRepositoryMock.VerifyNoOtherCalls();
        _optionRepositoryMock.VerifyNoOtherCalls();
        _unitOfWorkMock.VerifyNoOtherCalls();
    }
}
