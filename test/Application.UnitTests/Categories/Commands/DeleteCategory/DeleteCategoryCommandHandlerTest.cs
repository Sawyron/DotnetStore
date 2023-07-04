using Application.Categories;
using Application.Categories.Commands.CreateCategory;
using Application.Categories.Commands.DeleteCategory;
using Application.Core;
using Application.Exceptions.Categories;
using Application.Files;
using Application.Images;
using Domain.Categories;
using Domain.Files;
using Moq;

namespace Application.UnitTests.Categories.Commands.DeleteCategory;

public class DeleteCategoryCommandHandlerTest
{
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly Mock<IFileRepository> _fileRepositoryMock;
    private readonly Mock<IFileStorage> _fileStorageMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly CategoryIdMapper _mapper;

    public DeleteCategoryCommandHandlerTest()
    {
        _categoryRepositoryMock = new();
        _fileRepositoryMock = new();
        _fileStorageMock = new();
        _unitOfWorkMock = new();
        _mapper = new();
    }

    [Fact]
    public async Task Handle_Should_RemoveCategoryAndClearFiles()
    {
        // Arrange
        var categoryId = new CategoryId(new Guid("39BBB335-AADB-4AF3-A4AF-3FC6516564E3"));
        _categoryRepositoryMock.Setup(repo => repo.FindByIdAsync(categoryId))
            .ReturnsAsync(new Category(categoryId, "name"));
        var images = Enumerable.Range(1, 5)
            .Select(i => new StoredFile(
                new FileId(new Guid($"59D25F92-6128-4405-A848-65D4ED32198{i}")),
                $"path_{i}"))
            .ToList();
        _fileRepositoryMock.Setup(repo => repo.FindCategoryFiles(categoryId))
            .ReturnsAsync(images);
        var handler = new DeleteCategoryCommandHandler(
            _categoryRepositoryMock.Object,
            _fileRepositoryMock.Object,
            _fileStorageMock.Object,
            _mapper,
            _unitOfWorkMock.Object);
        // Act
        await handler.Handle(new DeleteCategoryCommand(categoryId), default);
        // Assert
        _categoryRepositoryMock.Verify(repo => repo.FindByIdAsync(categoryId), Times.Once());
        _categoryRepositoryMock.Verify(repo => repo.FindParentAsync(categoryId), Times.Once());
        _categoryRepositoryMock.Verify(repo => repo.DeleteAsync(categoryId), Times.Once());
        _categoryRepositoryMock.VerifyNoOtherCalls();
        _unitOfWorkMock.Verify(unit => unit.SaveChangesAsync(default), Times.Once());
        _fileStorageMock.Verify(storage => storage.DeleteFile(It.IsAny<StoredFile>()), Times.Exactly(images.Count));
    }

    [Fact]
    public async Task Handle_Should_Throw_CategoryNotFoundException_If_CategoryDoesNotExsit()
    {
        // Arrange
        var categoryId = new CategoryId(new Guid("39BBB335-AADB-4AF3-A4AF-3FC6516564E3"));
        var handler = new DeleteCategoryCommandHandler(
            _categoryRepositoryMock.Object,
            _fileRepositoryMock.Object,
            _fileStorageMock.Object,
            _mapper,
            _unitOfWorkMock.Object);
        // Act & Assert
        await Assert.ThrowsAsync<CategoryNotFoundException>(async () =>
        {
            await handler.Handle(new DeleteCategoryCommand(categoryId), default);
        });
        _categoryRepositoryMock.Verify(repo => repo.FindByIdAsync(categoryId), Times.Once());
    }

    [Fact]
    public async Task Handle_Should_UpdateChilderenParentIdToGrandParent()
    {
        // Arrange
        var categoryId = new CategoryId(new Guid("39BBB335-AADB-4AF3-A4AF-3FC6516564E3"));
        var parentId = new CategoryId(new Guid("8A6B9F7D-0A39-4CEE-9A0E-E88F03791562"));
        _categoryRepositoryMock.Setup(repo => repo.FindByIdAsync(categoryId))
            .ReturnsAsync(new Category(categoryId, "category_name"));
        _categoryRepositoryMock.Setup(repo => repo.FindParentAsync(categoryId))
            .ReturnsAsync(new Category(parentId, "parent_name"));
        var children = Enumerable.Range(1, 5)
            .Select(i => new Category(
                new CategoryId(new Guid($"C9268AEC-DA47-422A-B175-CE67455CC10{i}")),
                $"child_{i}"))
            .ToList();
        _categoryRepositoryMock.Setup(repo => repo.FindChildrenAsync(categoryId))
            .ReturnsAsync(children);
        _fileRepositoryMock.Setup(repo => repo.FindCategoryFiles(categoryId))
            .ReturnsAsync(new List<StoredFile>());
        var handler = new DeleteCategoryCommandHandler(
            _categoryRepositoryMock.Object,
            _fileRepositoryMock.Object,
            _fileStorageMock.Object,
            _mapper,
            _unitOfWorkMock.Object);
        // Act
        await handler.Handle(new DeleteCategoryCommand(categoryId), default);
        // Assert
        _categoryRepositoryMock.Verify(repo => repo.FindByIdAsync(categoryId), Times.Once());
        _categoryRepositoryMock.Verify(repo => repo.FindParentAsync(categoryId), Times.Once());
        _categoryRepositoryMock.Verify(repo => repo.UpdateParentIdAsync(categoryId, parentId), Times.Once());
        _categoryRepositoryMock.Verify(repo => repo.DeleteAsync(categoryId), Times.Once());
        _fileRepositoryMock.Verify(storage => storage.FindCategoryFiles(categoryId), Times.Once());
        _unitOfWorkMock.Verify(unit => unit.SaveChangesAsync(default), Times.Once());
        _fileStorageMock.VerifyNoOtherCalls();
    }
}
