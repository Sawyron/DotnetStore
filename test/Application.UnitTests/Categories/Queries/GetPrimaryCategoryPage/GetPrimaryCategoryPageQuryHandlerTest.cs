using Application.Categories;
using Application.Categories.Queries;
using Application.Categories.Queries.GetPrimaryCategoryPage;
using Application.Core;
using Domain.Categories;
using Moq;

namespace Application.UnitTests.Categories.Queries.GetPrimaryCategoryPage;

public class GetPrimaryCategoryPageQuryHandlerTest
{
    private readonly Mock<ICategoryRepository> _repositoryMock;
    private readonly CategoryItemResponseMapper _mapper;

    public GetPrimaryCategoryPageQuryHandlerTest()
    {
        _repositoryMock = new Mock<ICategoryRepository>();
        _mapper = new CategoryItemResponseMapper();
    }

    [Fact]
    public async Task Handle_Should_Work()
    {
        // Arrange
        var primaryCategories = Enumerable.Range(1, 5)
            .Select(i => new Category(new CategoryId(new Guid($"C0052721-0CD0-4121-A097-817AC2662A9{i}")), $"name_{i}"))
            .ToList();
        _repositoryMock.Setup(repo => repo.GetAllPrimaryAsync()).ReturnsAsync(primaryCategories);
        var handler = new GetPrimaryCategoryPageQueryHandler(_repositoryMock.Object, _mapper);
        // Act
        var response = await handler.Handle(new GetPageQuery<CategoryItemResponse>(0, 1), default);
        // Assert
        response.OrderBy(c => c.Name)
            .Zip(primaryCategories.Select(c => c.Map(_mapper)).OrderBy(c => c.Name))
            .ToList()
            .ForEach(pair => Assert.Equal(pair.First, pair.Second));
    }
}
