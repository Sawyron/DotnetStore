using Domain.Categories;
using Microsoft.EntityFrameworkCore;
using Persistence.Data.Categories;

namespace Persistence.UnitTests.Data.Categories;

public class CategoryRepositoryTest
{
    private readonly Fixtures _fixtures = new();
    private readonly CategoryDataMapper _mapper = new();

    [Fact]
    public async Task Add_Should_Work()
    {
        // Arrange
        using var contextFactory = new SqliteContextFactory();
        using var context = contextFactory.CreateContext();
        var id = new CategoryId(Guid.NewGuid());
        string categoryName = "name";
        var category = new Category(id, categoryName);
        var repository = new CategoryRepository(context, _mapper);
        // Act
        await repository.AddAsync(category, null);
        await context.SaveChangesAsync();
        // Assert
        CategoryData actualCategory = await context.Set<CategoryData>().FirstAsync(c => c.Id == id);
        Assert.Equal(categoryName, actualCategory.Name);
    }

    [Fact]
    public async Task Add_Should_Throw_InvalidOperationException_WhenIdExists()
    {
        // Arrange
        using var contextFactory = new SqliteContextFactory();
        using var context = contextFactory.CreateContext();
        CategoryData categoryData = _fixtures.Categories[0];
        var id = categoryData.Id;
        var category = new Category(id, "name");
        var repository = new CategoryRepository(context, _mapper);
        await context.AddAsync(categoryData);
        await context.SaveChangesAsync();
        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await repository.AddAsync(category, null);
        });
    }

    [Fact]
    public async Task FindByIdAsync_Should_Work()
    {
        // Arrange
        using var contextFactory = new SqliteContextFactory();
        using var context = contextFactory.CreateContext();
        var repository = new CategoryRepository(context, _mapper);
        CategoryData category = _fixtures.Categories[0];
        await context.AddAsync(category);
        await context.SaveChangesAsync();
        // Act
        Category? actualCategory = await repository.FindByIdAsync(category.Id);
        // Assert
        Assert.NotNull(actualCategory);
        CategoryData actualData = actualCategory.Map(_mapper);
        Assert.Equal(category.Id, actualData.Id);
        Assert.Equal(category.Name, actualData.Name);
    }

    [Fact]
    public async Task FindByIdAsync_Should_ReturnNull_IfNotExists()
    {
        // Arrange
        using var contextFactory = new SqliteContextFactory();
        using var context = contextFactory.CreateContext();
        var repository = new CategoryRepository(context, _mapper);
        var categoryId = new CategoryId(new Guid("D522CB8C-50CC-4F56-8C9B-A9229BCC67A0"));
        // Act
        Category? actualCategory = await repository.FindByIdAsync(categoryId);
        Assert.Null(actualCategory);
    }

    [Fact]
    public async Task FindByName_Should_Work()
    {
        // Arrange
        using var contextFactory = new SqliteContextFactory();
        using var context = contextFactory.CreateContext();
        var repository = new CategoryRepository(context, _mapper);
        CategoryData category = _fixtures.Categories[0];
        await context.AddAsync(category);
        await context.SaveChangesAsync();
        string name = category.Name;
        // Act
        Category? actualCategory = await repository.FindByNameAsync(name);
        Assert.NotNull(actualCategory);
        Assert.Equal(name, actualCategory.Map(_mapper).Name);
    }

    [Fact]
    public async Task FindByName_Should_ReturnNull_If_DoesNotExist()
    {
        // Arrange
        using var contextFactory = new SqliteContextFactory();
        using var context = contextFactory.CreateContext();
        var repository = new CategoryRepository(context, _mapper);
        string name = "category_name";
        // Act
        Category? actualCategory = await repository.FindByNameAsync(name);
        // Assert
        Assert.Null(actualCategory);
    }

    [Fact]
    public async Task FindChildren_Should_Work()
    {
        // Arrange
        using var contextFactory = new SqliteContextFactory();
        using var context = contextFactory.CreateContext();
        var repository = new CategoryRepository(context, _mapper);
        List<CategoryData> categories = _fixtures.Categories;
        CategoryData parent = categories[0];
        List<CategoryData> chidren = categories.Skip(1)
            .Take(10)
            .ToList();
        foreach (CategoryData child in chidren)
        {
            child.ParentId = parent.Id;
        }
        await context.AddRangeAsync(categories);
        await context.SaveChangesAsync();
        // Act
        var actualChildren = await repository.FindChildrenAsync(parent.Id);
        // Assert
        chidren.OrderBy(c => c.Name)
            .Zip(actualChildren.Select(c => c.Map(_mapper)).OrderBy(c => c.Name))
            .ToList()
            .ForEach(pair =>
            {
                Assert.Equal(pair.First.Id, pair.Second.Id);
                Assert.Equal(pair.First.Name, pair.Second.Name);
            });
    }

    [Fact]
    public async Task FindParent_Shoul_Work()
    {
        // Arrange
        using var contextFactory = new SqliteContextFactory();
        using var context = contextFactory.CreateContext();
        var repository = new CategoryRepository(context, _mapper);
        var category = _fixtures.Categories[0];
        var parent = _fixtures.Categories[1];
        category.ParentId = parent.Id;
        await context.AddRangeAsync(category, parent);
        await context.SaveChangesAsync();
        // Act
        var acutualParentData = (await repository.FindParentAsync(category.Id))?.Map(_mapper);
        // Assert
        Assert.NotNull(acutualParentData);
        Assert.Equal(category.ParentId, acutualParentData.Id);
        Assert.Equal(parent.Name, acutualParentData.Name);
    }

    [Fact]
    public async Task FindParent_Should_ReturnNull_If_CategoryIdDoesNotExit()
    {
        // Arrange
        using var contextFactory = new SqliteContextFactory();
        using var context = contextFactory.CreateContext();
        var repository = new CategoryRepository(context, _mapper);
        var categoryId = new CategoryId(new Guid("A1161E84-6547-41DC-B223-F75DBE417A47"));
        // Act
        Category? category = await repository.FindParentAsync(categoryId);
        // Assert
        Assert.Null(category);
    }

    [Fact]
    public async Task GetPage_Should_Work()
    {
        // Arrange
        using var contextFactory = new SqliteContextFactory();
        using var context = contextFactory.CreateContext();
        var repository = new CategoryRepository(context, _mapper);
        var categories = _fixtures.Categories;
        await context.AddRangeAsync(categories);
        await context.SaveChangesAsync();
        // Act
        int pageSize = 10;
        var page = (await repository.GetPageAsync(0, pageSize)).ToList();
        // Assert
        Assert.Equal(pageSize, page.Count);
        _fixtures.Categories
            .OrderBy(c => c.Name)
            .Take(pageSize)
            .Zip(page.Select(c => c.Map(_mapper)))
            .ToList()
            .ForEach(pair =>
            {
                Assert.Equal(pair.First.Id, pair.Second.Id);
                Assert.Equal(pair.First.Name, pair.Second.Name);
            });
    }

    [Fact]
    public async Task Update_Should_ReturnTrue_If_Success()
    {
        // Arrange
        using var contextFactory = new SqliteContextFactory();
        using var context = contextFactory.CreateContext();
        var repository = new CategoryRepository(context, _mapper);
        CategoryData category = _fixtures.Categories[0];
        CategoryData parent = _fixtures.Categories[1];
        CategoryId id = category.Id;
        CategoryId parentId = parent.Id;
        await context.SaveChangesAsync();
        await context.AddRangeAsync(category, parent);
        // Act
        string updatedName = "new_name";
        var categoryToUpdate = new Category(id, updatedName);
        bool result = await repository.UpdateAsync(categoryToUpdate, parentId);
        await context.SaveChangesAsync();
        // Assert
        Assert.True(result);
        CategoryData actualData = await context.Set<CategoryData>().FirstAsync(c => c.Id == id);
        await context.Entry(actualData).ReloadAsync();
        Assert.Equal(updatedName, actualData.Name);
        Assert.Equal(parentId, actualData.ParentId);
    }

    [Fact]
    public async Task Update_Should_Return_False_If_IdDoesNotExist()
    {
        // Arrange
        using var contextFactory = new SqliteContextFactory();
        using var context = contextFactory.CreateContext();
        var repository = new CategoryRepository(context, _mapper);
        var categoryId = new CategoryId(new Guid("DAC4E237-1A0E-45E5-B387-7788B0E0B2B4"));
        var updatedCategory = new Category(new CategoryId(new Guid("F62B5142-CBF3-418F-876A-A6BD2FC58674")), "name");
        // Act
        bool result = await repository.UpdateAsync(updatedCategory, null);
        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task UpdateParentId_Shoul_Work()
    {
        // Arrange
        using var contextFactory = new SqliteContextFactory();
        using var context = contextFactory.CreateContext();
        var repository = new CategoryRepository(context, _mapper);
        CategoryId parentId = _fixtures.Categories[0].Id;
        CategoryId updatedParentId = _fixtures.Categories[1].Id;
        var childeren = _fixtures.Categories
            .Skip(2)
            .Take(10)
            .ToList();
        foreach (var child in childeren)
        {
            child.ParentId = parentId;
        }
        await context.AddRangeAsync(_fixtures.Categories.Take(12));
        await context.SaveChangesAsync();
        // Act
        int categoriesAffected = await repository.UpdateParentIdAsync(parentId, updatedParentId);
        await context.SaveChangesAsync();
        // Assert
        Assert.Equal(childeren.Count, categoriesAffected);
        var actualChildren = await context.Set<CategoryData>()
            .Where(c => c.ParentId == updatedParentId)
            .ToListAsync();
        await Task.WhenAll(actualChildren.Select(c => context.Entry(c).ReloadAsync()));
        Assert.Equal(childeren.Count, actualChildren.Count);
        childeren.OrderBy(c => c.Name)
            .Zip(actualChildren.OrderBy(c => c.Name))
            .ToList()
            .ForEach(pair =>
            {
                (CategoryData expected, CategoryData actual) = pair;
                Assert.Equal(expected.Id, actual.Id);
                Assert.Equal(expected.Name, actual.Name);
                Assert.Equal(actual.ParentId, updatedParentId);
            });
    }

    [Fact]
    public async Task Delete_Should_ReturnTrueAndRemoveFromContext()
    {
        // Arrange
        using var contextFactory = new SqliteContextFactory();
        using var context = contextFactory.CreateContext();
        var repository = new CategoryRepository(context, _mapper);
        CategoryData category = _fixtures.Categories[0];
        await context.AddAsync(category);
        await context.SaveChangesAsync();
        var id = category.Id;
        // Act
        bool result = await repository.DeleteAsync(id);
        await context.SaveChangesAsync();
        // Assert
        Assert.True(result);
        CategoryData? actualData = await context.Set<CategoryData>().FindAsync(id);
        Assert.Null(actualData);
    }

    [Fact]
    public async Task Delete_Should_ReturnFalse_When_DoesNotExistInContext()
    {
        // Arrange
        using var contextFactory = new SqliteContextFactory();
        using var context = contextFactory.CreateContext();
        var repository = new CategoryRepository(context, _mapper);
        var id = new CategoryId(new Guid("9D4F817F-9DA0-4A99-A940-AE892FF3D228"));
        // Act
        bool result = await repository.DeleteAsync(id);
        await context.SaveChangesAsync();
        // Assert
        Assert.False(result);
    }
}
