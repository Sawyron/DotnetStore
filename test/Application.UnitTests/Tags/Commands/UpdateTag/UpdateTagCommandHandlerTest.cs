using Application.Categories;
using Application.Core;
using Application.Exceptions.Categories;
using Application.Exceptions.Tags;
using Application.Tags;
using Application.Tags.Commands.UpdateTag;
using Domain.Categories;
using Domain.ProductTypes.Details;
using Moq;

namespace Application.UnitTests.Tags.Commands.UpdateTag;

public class UpdateTagCommandHandlerTest
{
    private readonly Mock<ITagRepository> _tagRepositoryMock = new();
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    [Fact]
    public async Task Handle_Should_Work()
    {
        // Arrange
        var tagId = new TagId(new Guid("FE4C405E-C0AF-4528-A136-7ABEF05353A0"));
        var categoryId = new CategoryId(new Guid("E0EEE0DF-0C16-453A-B8FD-208FE45F00D5"));
        _tagRepositoryMock.Setup(repo => repo.FindByIdAsync(tagId))
            .ReturnsAsync(new Tag(tagId, "tag_name", categoryId));
        _categoryRepositoryMock.Setup(repo => repo.FindByIdAsync(categoryId))
            .ReturnsAsync(new Category(categoryId, "category_name"));
        var handler = new UpdateTagCommandHandler(
            _tagRepositoryMock.Object,
            _categoryRepositoryMock.Object,
            _unitOfWorkMock.Object);
        // Act
        await handler.Handle(new UpdateTagCommand(tagId, "name", categoryId), default);
        // Assert
        _tagRepositoryMock.Verify(repo => repo.FindByIdAsync(tagId), Times.Once());
        _categoryRepositoryMock.Verify(repo => repo.FindByIdAsync(categoryId), Times.Once());
        _tagRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Tag>()), Times.Once());
        _unitOfWorkMock.Verify(unit => unit.SaveChangesAsync(default), Times.Once());
    }

    [Fact]
    public async Task Handle_Should_Throw_TagNotFoundException_If_TagDoesNotExist()
    {
        // Arrange
        var tagId = new TagId(new Guid("FE4C405E-C0AF-4528-A136-7ABEF05353A0"));
        var handler = new UpdateTagCommandHandler(
            _tagRepositoryMock.Object,
            _categoryRepositoryMock.Object,
            _unitOfWorkMock.Object);
        // Act & Assert
        await Assert.ThrowsAsync<TagNotFoundException>(async () =>
        {
            await handler.Handle(new UpdateTagCommand(tagId, "name", new CategoryId(new Guid())), default);
        });
        _tagRepositoryMock.Verify(repo => repo.FindByIdAsync(tagId), Times.Once());
        _tagRepositoryMock.VerifyNoOtherCalls();
        _categoryRepositoryMock.VerifyNoOtherCalls();
        _unitOfWorkMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Handle_Should_Throw_CategoryNotFoundException_If_CategoryDoesNotExist()
    {
        // Arrange
        var tagId = new TagId(new Guid("FE4C405E-C0AF-4528-A136-7ABEF05353A0"));
        var categoryId = new CategoryId(new Guid("D0452C76-6F08-403C-BDEF-69BA9079F1DE"));
        _tagRepositoryMock.Setup(repo => repo.FindByIdAsync(tagId))
            .ReturnsAsync(new Tag(tagId, "name", categoryId));
        var handler = new UpdateTagCommandHandler(
            _tagRepositoryMock.Object,
            _categoryRepositoryMock.Object,
            _unitOfWorkMock.Object);
        // Act & Assert
        await Assert.ThrowsAsync<CategoryNotFoundException>(async () =>
        {
            await handler.Handle(new UpdateTagCommand(tagId, "name", categoryId), default);
        });
        _tagRepositoryMock.Verify(repo => repo.FindByIdAsync(tagId), Times.Once());
        _categoryRepositoryMock.Verify(repo => repo.FindByIdAsync(categoryId));
        _tagRepositoryMock.VerifyNoOtherCalls();
        _categoryRepositoryMock.VerifyNoOtherCalls();
        _unitOfWorkMock.VerifyNoOtherCalls();
    }
}
