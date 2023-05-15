using Application.Categories;
using Application.Categories.Queries;
using Application.Categories.Queries.GetCategoryById;
using Application.Exceptions.Categories;
using Domain.Categories;
using Moq;

namespace Application.UnitTests.Categories.Queries.GetCategoryBuId;

public class GetCategoryByIdQueryHandlerTest
{
    private readonly Mock<ICategoryRepository> _repositoryMock;
    private readonly CategoryResponseMapper _responseMapper;
    private readonly CategoryItemResponseMapper _childMapper;

    public GetCategoryByIdQueryHandlerTest()
    {
        _repositoryMock = new Mock<ICategoryRepository>();
        _responseMapper = new CategoryResponseMapper();
        _childMapper = new CategoryItemResponseMapper();
    }

    [Fact]
    public async Task Hanlde_Should_ReturnResponseWithEmptyChildren()
    {
        // Arrange
        var id = new CategoryId(new Guid("E704D37E-3483-4621-AE62-AB775C0F42A6"));
        string name = "category_name";
        var category = new Category(id, name);
        _repositoryMock.Setup(repo => repo.FindByIdAsync(id)).ReturnsAsync(category);
        _repositoryMock.Setup(repo => repo.FindChildrenAsync(id)).ReturnsAsync(Array.Empty<Category>());
        var handelr = new GetCategoryByIdQueryHandler(_repositoryMock.Object, _responseMapper, _childMapper);
        // Act
        var response = await handelr.Handle(new GetCategoryByIdQuery(id), default);
        // Assert
        Assert.Equal(id.Value, response.Id);
        Assert.Equal(name, response.Name);
        Assert.Empty(response.Children);
        _repositoryMock.Verify(repo => repo.FindByIdAsync(id), Times.Once());
        _repositoryMock.Verify(repo => repo.FindChildrenAsync(id), Times.Once());
    }

    [Fact]
    public async Task Hanlde_Should_ReturnResponseWithChildren()
    {
        // Arrange
        var id = new CategoryId(new Guid("E704D37E-3483-4621-AE62-AB775C0F42A6"));
        string name = "category_name";
        var category = new Category(id, name);
        var children = Enumerable.Range(1, 5)
            .Select(i => new Category(
                new CategoryId(new Guid($"E704D37E-3483-4621-AE62-AB775C0F42A{i % 10}")),
                $"category_name_{i}"))
            .ToList();
        _repositoryMock.Setup(repo => repo.FindByIdAsync(id)).ReturnsAsync(category);
        _repositoryMock.Setup(repo => repo.FindChildrenAsync(id)).ReturnsAsync(children);
        var handelr = new GetCategoryByIdQueryHandler(_repositoryMock.Object, _responseMapper, _childMapper);
        // Act
        var response = await handelr.Handle(new GetCategoryByIdQuery(id), default);
        // Assert
        Assert.Equal(id.Value, response.Id);
        Assert.Equal(name, response.Name);
        Assert.Equal(children.Count, response.Children.Count());
        response.Children
            .OrderBy(c => c.Name)
            .Zip(children.Select(c => c.Map(_childMapper))
                .OrderBy(c => c.Name))
            .ToList()
            .ForEach(pair => Assert.Equal(pair.First, pair.Second));
        _repositoryMock.Verify(repo => repo.FindByIdAsync(id), Times.Once());
        _repositoryMock.Verify(repo => repo.FindChildrenAsync(id), Times.Once());
    }

    [Fact]
    public async Task Handle_Should_ThrowCategoryNotFoundException_WhenCategoryDoesNotExist()
    {
        // Arrange
        var id = new CategoryId(new Guid("E704D37E-3483-4621-AE62-AB775C0F42A6"));
        _repositoryMock.Setup(repo => repo.FindByIdAsync(id)).ReturnsAsync((Category)null!);
        var handelr = new GetCategoryByIdQueryHandler(_repositoryMock.Object, _responseMapper, _childMapper);
        // Act & Assert
        await Assert.ThrowsAsync<CategoryNotFoundException>(async () =>
        {
            await handelr.Handle(new GetCategoryByIdQuery(id), default);
        });
    }
}
