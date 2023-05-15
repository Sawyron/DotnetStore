using Application.Categories;
using Application.Core;
using Application.Exceptions.Categories;
using Application.Exceptions.Tags;
using Application.Tags;
using Application.Tags.Commands.CreateTag;
using Domain.Categories;
using Domain.ProductTypes.Details;
using Domain.Tags;
using Moq;

namespace Application.UnitTests.Tags.Commands.CreateTag;

public class CreateTagCommandHandlerTest
{
    private readonly Mock<ITagRepository> _tagRepositoryMock;
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public CreateTagCommandHandlerTest()
    {
        _tagRepositoryMock = new();
        _categoryRepositoryMock = new();
        _unitOfWorkMock = new();
    }

    [Fact]
    public async Task Handle_Should_Work()
    {
        // Arrange
        var categoryId = new CategoryId(new Guid("978286D0-A766-41AB-A683-BFB006C17A92"));
        var tagName = "tag_name";
        _categoryRepositoryMock.Setup(repo => repo.FindByIdAsync(categoryId))
            .ReturnsAsync(new Category(categoryId, "category_name"));
        _tagRepositoryMock.Setup(repo => repo.FindByNameInCategory(tagName, categoryId))
            .ReturnsAsync((Tag)null!);
        var hanlder = new CreateTagCommandHandler(
            _tagRepositoryMock.Object,
            _categoryRepositoryMock.Object,
            _unitOfWorkMock.Object);
        // Act
        await hanlder.Handle(new CreateTagCommand(tagName, categoryId), default);
        // Assert
        _categoryRepositoryMock.Verify(repo => repo.FindByIdAsync(categoryId), Times.Once());
        _tagRepositoryMock.Verify(repo => repo.FindByNameInCategory(tagName, categoryId), Times.Once());
        _tagRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Tag>()), Times.Once());
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once());
    }

    [Fact]
    public void Handle_Should_Thorow_CategoryNotFoundException_When_CategoryDoesNotExist()
    {
        // Arrange
        var categoryId = new CategoryId(new Guid("978286D0-A766-41AB-A683-BFB006C17A92"));
        _categoryRepositoryMock.Setup(repo => repo.FindByIdAsync(categoryId))
            .ReturnsAsync((Category)null!);
        var hanlder = new CreateTagCommandHandler(
            _tagRepositoryMock.Object,
            _categoryRepositoryMock.Object,
            _unitOfWorkMock.Object);
        // Act & Assert
        Assert.ThrowsAsync<CategoryNotFoundException>(async () =>
        {
            await hanlder.Handle(new CreateTagCommand(string.Empty, categoryId), default);
        });
        _categoryRepositoryMock.Verify(repo => repo.FindByIdAsync(categoryId), Times.Once());
        _tagRepositoryMock.VerifyNoOtherCalls();
        _unitOfWorkMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Handle_Should_Thorow_TagNameConflictException_When_TagWithSuchNameExists()
    {
        // Arrange
        var categoryId = new CategoryId(new Guid("978286D0-A766-41AB-A683-BFB006C17A92"));
        var tagName = "tag_name";
        _categoryRepositoryMock.Setup(repo => repo.FindByIdAsync(categoryId))
            .ReturnsAsync(new Category(categoryId, string.Empty));
        _tagRepositoryMock.Setup(repo => repo.FindByNameInCategory(tagName, categoryId))
            .ReturnsAsync(new Tag(new TagId(new Guid("FA21B978-AD15-4A6B-99E3-398E0A5D7118")), tagName, categoryId));
        var hanlder = new CreateTagCommandHandler(
            _tagRepositoryMock.Object,
            _categoryRepositoryMock.Object,
            _unitOfWorkMock.Object);
        // Act & Assert
        await Assert.ThrowsAsync<TagNameConflictException>(async () =>
        {
            await hanlder.Handle(new CreateTagCommand(tagName, categoryId), default);
        });
        _categoryRepositoryMock.Verify(repo => repo.FindByIdAsync(categoryId), Times.Once());
        _tagRepositoryMock.Verify(repo => repo.FindByNameInCategory(tagName, categoryId), Times.Once());
        _tagRepositoryMock.VerifyNoOtherCalls();
        _unitOfWorkMock.VerifyNoOtherCalls();
    }
}
