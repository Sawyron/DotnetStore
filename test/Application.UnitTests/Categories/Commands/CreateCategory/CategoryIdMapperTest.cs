using Application.Categories.Commands.CreateCategory;
using Domain.Categories;

namespace Application.UnitTests.Categories.Commands.CreateCategory;

public class CategoryIdMapperTest
{
    private readonly CategoryIdMapper _idMapper;

    public CategoryIdMapperTest()
    {
        _idMapper = new CategoryIdMapper();
    }

    [Fact]
    public void Map_Should_Work()
    {
        // Arragne
        var id = new CategoryId(new Guid("9625F2B4-FFFB-4F17-ACC1-B911E317F32D"));
        var category = new Category(id, "name");
        // Act
        var actualId = category.Map(_idMapper);
        // Assert
        Assert.Equal(id, actualId);
    }
}
