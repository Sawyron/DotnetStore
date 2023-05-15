using Domain.Categories;
using Domain.Tags;
using Microsoft.EntityFrameworkCore;
using Persistence.Data.Categories;
using Persistence.Data.Tags;

namespace Persistence.UnitTests.Data.Tags;

public class TagRepositoryTest
{
    private readonly Fixtures _fixtures = new();
    private readonly TagDataMapper _mapper = new();

    [Fact]
    public async Task Add_Should_Work()
    {
        // Arrange
        using var contextFactory = new SqliteContextFactory();
        using var context = contextFactory.CreateContext();
        var repository = new TagRepository(context, _mapper);
        var id = new TagId(new Guid("FABB0084-8A33-4821-8E41-53864AEE503D"));
        var name = "tag_name";
        CategoryData categoryData = _fixtures.Categories[0];
        CategoryId categoryId = categoryData.Id;
        await context.AddAsync(categoryData);
        await context.SaveChangesAsync();
        // Act
        var tag = new Tag(id, name, categoryId);
        await repository.AddAsync(tag);
        await context.SaveChangesAsync();
        // Assert
        TagData actualTag = await context.Set<TagData>().FirstAsync(t => t.Id == id);
        Assert.Equal(name, actualTag.Name);
        Assert.Equal(categoryId, actualTag.CategoryId);
    }

    [Fact]
    public async Task Add_Should_Throw_InvalidOperatinException_If_IdExists()
    {
        // Arrange
        using var contextFactory = new SqliteContextFactory();
        using var context = contextFactory.CreateContext();
        var repository = new TagRepository(context, _mapper);
        TagData tagData = _fixtures.Tags[0];
        var id = tagData.Id;
        var name = "tag_name";
        CategoryData categoryData = tagData.Category;
        CategoryId categoryId = categoryData.Id;
        await context.AddAsync(categoryData);
        await context.AddAsync(tagData);
        await context.SaveChangesAsync();
        // Act
        var tag = new Tag(id, name, categoryId);
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await repository.AddAsync(tag);
        });
    }

    [Fact]
    public async Task Add_Should_Throw_DbUpdateException_When_CategoryIdDoesNotExist()
    {
        // Arrange
        using var contextFactory = new SqliteContextFactory();
        using var context = contextFactory.CreateContext();
        var repository = new TagRepository(context, _mapper);
        CategoryId categoryId = _fixtures.Categories[0].Id;
        var tag = new Tag(new TagId(Guid.NewGuid()), "name", categoryId);
        // Act & Assert
        await Assert.ThrowsAsync<DbUpdateException>(async () =>
        {
            await repository.AddAsync(tag);
            await context.SaveChangesAsync();
        });
    }

    [Fact]
    public async Task AddRange_Should_Work()
    {
        // Arrange
        using var contextFactory = new SqliteContextFactory();
        using var context = contextFactory.CreateContext();
        var repository = new TagRepository(context, _mapper);
        var category = _fixtures.Categories[0];
        CategoryId categoryId = category.Id;
        await context.AddAsync(category);
        await context.SaveChangesAsync();
        List<Tag> tags = Enumerable.Range(1, 5)
            .Select(i => new Tag(
                new TagId(new Guid($"DF0488C2-9E17-4BC2-95B2-2242D29E74{i}C")),
                $"tag_name_{i}",
                categoryId))
            .ToList();
        // Act
        await repository.AddRangeAsync(tags);
        await context.SaveChangesAsync();
        // Assert
        var actualTagsData = await context.Set<TagData>()
            .OrderBy(t => t.Name)
            .ToListAsync();
        Assert.Equal(tags.Count, actualTagsData.Count);
        tags.Select(t => t.Map(_mapper))
            .OrderBy(t => t.Name)
            .Zip(actualTagsData)
            .ToList()
            .ForEach(pair =>
            {
                Assert.Equal(pair.First.Id, pair.Second.Id);
                Assert.Equal(pair.First.Name, pair.Second.Name);
                Assert.Equal(pair.First.CategoryId, pair.Second.CategoryId);
            });
    }

    [Fact]
    public async Task FindById_Should_Work()
    {
        // Arrange
        using var contextFactory = new SqliteContextFactory();
        using var context = contextFactory.CreateContext();
        var repository = new TagRepository(context, _mapper);
        TagData tagData = _fixtures.Tags[0];
        TagId tagId = tagData.Id;
        await context.AddAsync(tagData);
        await context.SaveChangesAsync();
        // Act
        TagData? actualTagData = (await repository.FindByIdAsync(tagId))?.Map(_mapper);
        // Assert
        Assert.NotNull(actualTagData);
        Assert.Equal(tagData.Id, actualTagData.Id);
        Assert.Equal(tagData.Name, actualTagData.Name);
        Assert.Equal(tagData.CategoryId, actualTagData.CategoryId);
    }

    [Fact]
    public async Task FindById_Should_ReturnNull_If_IdDoesNotExist()
    {
        // Arrange
        using var contextFactory = new SqliteContextFactory();
        using var context = contextFactory.CreateContext();
        var repository = new TagRepository(context, _mapper);
        var tagId = new TagId(new Guid("CB000465-CC1F-4196-891F-C1C98881A722"));
        // Act
        Tag? actualTag = await repository.FindByIdAsync(tagId);
        Assert.Null(actualTag);
    }

    [Fact]
    public async Task FindByNameInCategory_Should_Work()
    {
        // Arrange
        using var contextFactory = new SqliteContextFactory();
        using var context = contextFactory.CreateContext();
        var repository = new TagRepository(context, _mapper);
        TagData tagData = _fixtures.Tags[0];
        string name = tagData.Name;
        CategoryId categoryId = tagData.CategoryId;
        await context.AddAsync(tagData);
        await context.SaveChangesAsync();
        // Act
        TagData? actualTagData = (await repository.FindByNameInCategory(name, categoryId))?.Map(_mapper);
        Assert.NotNull(actualTagData);
        Assert.Equal(categoryId, actualTagData.CategoryId);
        Assert.Equal(name, actualTagData.Name);
        Assert.Equal(categoryId, actualTagData.CategoryId);
    }

    [Fact]
    public async Task FindByNameInCategory_Should_ReturnNull_If_IdDoesNotExist()
    {
        // Arrange
        using var contextFactory = new SqliteContextFactory();
        using var context = contextFactory.CreateContext();
        var repository = new TagRepository(context, _mapper);
        var tagId = new TagId(new Guid("62609610-051A-451E-95ED-8E6357345E87"));
        // Act
        Tag? actualTag = await repository.FindByNameInCategory("name", _fixtures.Categories[0].Id);
        Assert.Null(actualTag);
    }

    [Fact]
    public async Task FindByIds_Should_Work()
    {
        // Arrange
        using var contextFactory = new SqliteContextFactory();
        using var context = contextFactory.CreateContext();
        var repository = new TagRepository(context, _mapper);
        var tags = _fixtures.Tags;
        var ids = tags.Select(t => t.Id).ToList();
        await context.AddRangeAsync(tags);
        await context.SaveChangesAsync();
        // Act
        var actualTagsData = (await repository.FindByIdsAsync(ids))
            .Select(t => t.Map(_mapper))
            .ToList();
        Assert.Equal(tags.Count, actualTagsData.Count);
        tags.OrderBy(t => t.Name)
            .Zip(actualTagsData.OrderBy(t => t.Name))
            .ToList()
            .ForEach(pair =>
            {
                Assert.Equal(pair.First.Id, pair.Second.Id);
                Assert.Equal(pair.First.Name, pair.Second.Name);
                Assert.Equal(pair.First.CategoryId, pair.Second.CategoryId);
            });
    }

    [Fact]
    public async Task FindCategoryTags_Shoudl_Work()
    {
        // Arrange
        using var contextFactory = new SqliteContextFactory();
        using var context = contextFactory.CreateContext();
        var repository = new TagRepository(context, _mapper);
        CategoryId categoryId = _fixtures.Categories[0].Id;
        var tagsData = _fixtures.Tags.Where(t => t.CategoryId == categoryId).ToList();
        await context.AddRangeAsync(tagsData);
        await context.SaveChangesAsync();
        // Act
        var actualTagsData = (await repository.FindCategoryTags(categoryId))
            .Select(t => t.Map(_mapper))
            .ToList();
        Assert.Equal(tagsData.Count, actualTagsData.Count);
        tagsData.OrderBy(t => t.Name)
            .Zip(actualTagsData.OrderBy(t => t.Name))
            .ToList()
            .ForEach(pair =>
            {
                Assert.Equal(pair.First.Id, pair.Second.Id);
                Assert.Equal(pair.First.Name, pair.Second.Name);
                Assert.Equal(categoryId, pair.Second.CategoryId);
            });
    }

    [Fact]
    public async Task Delete_Should_ReturnTrue_If_Success()
    {
        // Arrange
        using var contextFactory = new SqliteContextFactory();
        using var context = contextFactory.CreateContext();
        var repository = new TagRepository(context, _mapper);
        TagData tag = _fixtures.Tags[0];
        TagId tagId = tag.Id;
        await context.AddAsync(tag);
        await context.SaveChangesAsync();
        // Act
        bool result = await repository.DeleteAsync(tagId);
        await context.SaveChangesAsync();
        Assert.True(result);
        TagData? actualTagData = await context.Set<TagData>()
            .FindAsync(tagId);
        Assert.Null(actualTagData);
    }

    [Fact]
    public async Task Delete_Should_ReturnFalse_If_Fail()
    {
        // Arrange
        using var contextFactory = new SqliteContextFactory();
        using var context = contextFactory.CreateContext();
        var repository = new TagRepository(context, _mapper);
        var tagId = new TagId(new Guid("939F1EC9-D117-4C6A-B3A5-FEBA54B44EB0"));
        // Act
        bool result = await repository.DeleteAsync(tagId);
        Assert.False(result);
    }

    [Fact]
    public async Task Update_Should_ReturnTrue_If_Success()
    {
        // Arrange
        using var contextFactory = new SqliteContextFactory();
        using var context = contextFactory.CreateContext();
        var repository = new TagRepository(context, _mapper);
        TagData tagData = _fixtures.Tags[0];
        await context.AddAsync(tagData);
        await context.AddAsync(_fixtures.Categories[1]);
        await context.SaveChangesAsync();
        string updatedName = "new_name";
        var updatedTag = new Tag(tagData.Id, updatedName, _fixtures.Categories[1].Id);
        // Act
        bool result = await repository.UpdateAsync(updatedTag);
        await context.SaveChangesAsync();
        // Assert
        Assert.True(result);
        TagData actualTagData = await context.Set<TagData>().FirstAsync(t => t.Id == tagData.Id);
        await context.Entry(actualTagData).ReloadAsync();
        Assert.Equal(updatedName, actualTagData.Name);
        Assert.Equal(_fixtures.Categories[1].Id, actualTagData.CategoryId);
    }

    [Fact]
    public async Task Update_Should_ReturnFalse_If_IdDoesNotExist()
    {
        // Arrange
        using var contextFactory = new SqliteContextFactory();
        using var context = contextFactory.CreateContext();
        var repository = new TagRepository(context, _mapper);
        // Act
        bool result = await repository.UpdateAsync(new Tag(
            new TagId(new Guid("9D217750-E138-4133-9A94-7B04C1A6E4A9")),
            "name",
            _fixtures.Categories[0].Id));
        // Assert
        Assert.False(result);
    }
}
