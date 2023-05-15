using Application.Categories;
using Application.Categories.Commands.UpdateCategory;
using Application.Core;
using Application.Exceptions.Categories;
using Domain.Categories;
using Moq;

namespace Application.UnitTests.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommandHandlerTest
{
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public UpdateCategoryCommandHandlerTest()
    {
        _categoryRepositoryMock = new();
        _unitOfWorkMock = new();
    }

    [Fact]
    public async Task Handle_Should_Work()
    {
        // Arrange
        var categoryId = new CategoryId(new Guid("B43929C5-08B7-4C78-A78A-8383D8FB7E2A"));
        string updatedName = "new_name";
        var updatedParentId = new CategoryId(new Guid("0E8FD227-2260-4AAB-9D15-55E26C252449"));
        _categoryRepositoryMock.Setup(repo => repo.FindByIdAsync(categoryId))
            .ReturnsAsync(new Category(categoryId, "name"));
        _categoryRepositoryMock.Setup(repo => repo.FindByIdAsync(updatedParentId))
            .ReturnsAsync(new Category(updatedParentId, "parent_name"));
        var handler = new UpdateCategoryCommandHandler(
            _categoryRepositoryMock.Object,
            _unitOfWorkMock.Object);
        // Act
        await handler.Handle(new UpdateCategoryCommand(categoryId, updatedName, updatedParentId), default);
        // Assert
        _categoryRepositoryMock.Verify(repo => repo.FindByIdAsync(categoryId), Times.Once());
        _categoryRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Category>(), updatedParentId), Times.Once());
        _unitOfWorkMock.Verify(unit => unit.SaveChangesAsync(default), Times.Once());
    }

    [Fact]
    public async Task Handle_Should_Throw_CategoryNotFoundException_If_CategoryDoesNotExist()
    {
        // Arrange
        var categoryId = new CategoryId(new Guid("B43929C5-08B7-4C78-A78A-8383D8FB7E2A"));
        var handler = new UpdateCategoryCommandHandler(
            _categoryRepositoryMock.Object,
            _unitOfWorkMock.Object);
        // Act & Assert
        await Assert.ThrowsAsync<CategoryNotFoundException>(async () =>
        {
            await handler.Handle(new UpdateCategoryCommand(categoryId, "name", null), default);
        });
        _categoryRepositoryMock.Verify(repo => repo.FindByIdAsync(categoryId), Times.Once());
        _categoryRepositoryMock.VerifyNoOtherCalls();
        _unitOfWorkMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Hanlde_Should_Throw_CategoryNotFoundException_If_ParentDoesNotExist()
    {
        // Arrange
        var categoryId = new CategoryId(new Guid("B43929C5-08B7-4C78-A78A-8383D8FB7E2A"));
        _categoryRepositoryMock.Setup(repo => repo.FindByIdAsync(categoryId))
            .ReturnsAsync(new Category(categoryId, "name"));
        var parentId = new CategoryId(new Guid("CC9B3980-6AFB-4A49-AA62-ECC4218C9996"));
        var handler = new UpdateCategoryCommandHandler(
            _categoryRepositoryMock.Object,
            _unitOfWorkMock.Object);
        // Act & Assert
        await Assert.ThrowsAsync<CategoryNotFoundException>(async () =>
        {
            await handler.Handle(new UpdateCategoryCommand(categoryId, "name", parentId), default);
        });
        _categoryRepositoryMock.Verify(repo => repo.FindByIdAsync(categoryId), Times.Once());
        _categoryRepositoryMock.Verify(repo => repo.FindByIdAsync(parentId), Times.Once());
        _categoryRepositoryMock.VerifyNoOtherCalls();
        _unitOfWorkMock.VerifyNoOtherCalls();
    }
}
