using Application.Categories;
using Application.Categories.Commands.CreateCategory;
using Application.Core;
using Application.Exceptions.Categories;
using Domain.Categories;
using Moq;

namespace Application.UnitTests.Categories.Commands.CreateCategory;

public class CreateCategoryCommandHandlerTest
{
    private readonly Mock<ICategoryRepository> _repositoryMock;
    private readonly CategoryIdMapper _mapper;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public CreateCategoryCommandHandlerTest()
    {
        _repositoryMock = new Mock<ICategoryRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapper = new CategoryIdMapper();
    }

    [Fact]
    public async Task Handle_Should_CallMapperAndRepo()
    {
        // Arrange
        var expectedName = "expectedName";
        var categoryId = new CategoryId(new Guid("9625F2B4-FFFB-4F17-ACC1-B911E317F32D"));
        _repositoryMock.Setup(repo => repo.FindByNameAsync(expectedName))
            .ReturnsAsync((Category)null!);
        _repositoryMock.Setup(repo => repo.FindByIdAsync(It.IsAny<CategoryId>()))
            .ReturnsAsync(new Category(categoryId, "expectedName"));
        var handler = new CreateCategoryCommandHandler(_repositoryMock.Object, _mapper, _unitOfWorkMock.Object);
        // Act
        await handler.Handle(new CreateCategoryCommand(expectedName, null), default);
        // Assert
        _repositoryMock.Verify(repo => repo.FindByNameAsync(expectedName), Times.Once());
        _repositoryMock.Verify(repo => repo.FindByIdAsync(It.IsAny<CategoryId>()), Times.Never());
        _repositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Category>(), null), Times.Once());
    }

    [Fact]
    public async Task Handle_Should_ThrowCategoryNameDuplicatedException_When_NameAlreadyExists()
    {
        var name = "expectedName";
        var id = new Category(new CategoryId(new Guid("9625F2B4-FFFB-4F17-ACC1-B911E317F32D")), "expectedName");
        _repositoryMock.Setup(repo => repo.FindByNameAsync(name))
            .ReturnsAsync(id);
        var handler = new CreateCategoryCommandHandler(_repositoryMock.Object, _mapper, _unitOfWorkMock.Object);
        // Act
        await Assert.ThrowsAsync<CategoryNameDuplicatedException>(async () =>
        {
            await handler.Handle(new CreateCategoryCommand(name, null), default);
        });
    }
}
