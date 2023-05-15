using Application.Categories.Queries;
using Application.Categories.Queries.GetPrimaryCategoryPage;
using Domain.Categories;

namespace Application.UnitTests.Categories.Queries;

public class CategoryItemResponseMapperTest
{
    private readonly CategoryItemResponseMapper _mapper;

    public CategoryItemResponseMapperTest()
    {
        _mapper = new CategoryItemResponseMapper();
    }

    [Fact]
    public void Map_Should_Work()
    {
        // Arragne
        var id = new CategoryId(new Guid("9625F2B4-FFFB-4F17-ACC1-B911E317F32D"));
        string name = "name";
        var category = new Category(id, name);
        // Act
        CategoryItemResponse response = category.Map(_mapper);
        // Assert
        Assert.Equal(id.Value, response.Id);
        Assert.Equal(name, response.Name);
    }
}
