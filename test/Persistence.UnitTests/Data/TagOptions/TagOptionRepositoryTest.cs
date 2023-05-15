using Domain.Categories;
using Domain.Products;
using Domain.ProductTypes.Details;
using Domain.ProductTypes.Tags.TagOptions;
using Microsoft.EntityFrameworkCore;
using Persistence.Data.Products;
using Persistence.Data.TagOptions;
using Persistence.Data.Tags;

namespace Persistence.UnitTests.Data.TagOptions;

public class TagOptionRepositoryTest
{
    private readonly Fixtures _fixtures = new();
    private readonly TagOptionDataMapper _mapper = new();

    [Fact]
    public async Task Add_Should_Work()
    {
        // Arrange
        using var contextFactory = new SqliteContextFactory();
        using var context = contextFactory.CreateContext();
        var repository = new TagOptionRepository(context, _mapper);
        var optionId = new TagOptionId(new Guid("F46DCEB8-9F9F-4A0A-B846-5B1C4489D240"));
        string value = "option_name";
        TagData tagData = _fixtures.Tags[0];
        TagId tagId = tagData.Id;
        var option = new TagOption(optionId, value, tagId);
        await context.AddAsync(tagData);
        await context.SaveChangesAsync();
        // Act
        await repository.AddAsync(option);
        await context.SaveChangesAsync();
        // Assert
        TagOptionData actualTagOptionData = await context.Set<TagOptionData>().FirstAsync(o => o.Id == optionId);
        Assert.Equal(optionId, actualTagOptionData.Id);
        Assert.Equal(value, actualTagOptionData.Value);
        Assert.Equal(tagData.Id, actualTagOptionData.TagId);
    }

    [Fact]
    public async Task Add_Should_Thorow_InvalidOperationException_When_IdExists()
    {
        // Arrange
        using var contextFactory = new SqliteContextFactory();
        using var context = contextFactory.CreateContext();
        var repository = new TagOptionRepository(context, _mapper);
        TagOptionData optionData = _fixtures.TagOptions[0];
        TagOptionId optionId = optionData.Id;
        await context.AddAsync(optionData);
        await context.SaveChangesAsync();
        var option = new TagOption(optionId, "value", optionData.TagId);
        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await repository.AddAsync(option);
        });
    }

    [Fact]
    public async Task Add_Should_Throw_DbUpdateException_When_TagIdDoesNotExist()
    {
        // Arrange
        using var contextFactory = new SqliteContextFactory();
        using var context = contextFactory.CreateContext();
        var repository = new TagOptionRepository(context, _mapper);
        var option = new TagOption(
            new TagOptionId(new Guid("77F6FC11-7893-4ED0-8A95-0A0BBE7D0CAD")),
            "name",
            new TagId(new Guid("48DC7B5F-D403-4673-A6B2-E9063869C714")));
        // Act & Assert
        await Assert.ThrowsAsync<DbUpdateException>(async () =>
        {
            await repository.AddAsync(option);
            await context.SaveChangesAsync();
        });
    }

    [Fact]
    public async Task FindById_Should_Work()
    {
        // Arrange
        using var contextFactory = new SqliteContextFactory();
        using var context = contextFactory.CreateContext();
        var repository = new TagOptionRepository(context, _mapper);
        TagOptionData optionData = _fixtures.TagOptions[0];
        TagOptionId optionId = optionData.Id;
        await context.AddAsync(optionData);
        await context.SaveChangesAsync();
        // Act
        TagOptionData? actualOptionData = (await repository.FindByIdAsync(optionId))?.Map(_mapper);
        Assert.NotNull(actualOptionData);
        Assert.Equal(optionData.Id, actualOptionData.Id);
        Assert.Equal(optionData.Value, actualOptionData.Value);
        Assert.Equal(optionData.TagId, actualOptionData.TagId);
    }

    [Fact]
    public async Task FindById_Should_RetunNull_If_IdDoesNotExist()
    {
        // Arrange
        using var contextFactory = new SqliteContextFactory();
        using var context = contextFactory.CreateContext();
        var repository = new TagOptionRepository(context, _mapper);
        var optionId = new TagOptionId(new Guid("E8183632-FF38-43A0-80C3-BCC3863B69FA"));
        // Act
        TagOption? option = await repository.FindByIdAsync(optionId);
        Assert.Null(option);
    }

    [Fact]
    public async Task FindByIds_Should_Work()
    {
        // Arrange
        using var contextFactory = new SqliteContextFactory();
        using var context = contextFactory.CreateContext();
        var repository = new TagOptionRepository(context, _mapper);
        var optionsData = _fixtures.TagOptions;
        await context.AddRangeAsync(optionsData);
        await context.SaveChangesAsync();
        // Act
        var actualOptionsData = (await repository.FindByIds(optionsData.Select(o => o.Id)))
            .Select(o => o.Map(_mapper))
            .ToList();
        // Assert
        Assert.Equal(optionsData.Count, actualOptionsData.Count);
        optionsData.OrderBy(o => o.Value)
            .Zip(actualOptionsData.OrderBy(o => o.Value))
            .ToList()
            .ForEach(pair =>
            {
                Assert.Equal(pair.First.Id, pair.Second.Id);
                Assert.Equal(pair.First.Value, pair.Second.Value);
                Assert.Equal(pair.First.TagId, pair.Second.TagId);
            });
    }

    [Fact]
    public async Task GetTagOptionsPage_Should_Work()
    {
        // Arrange
        using var contextFactory = new SqliteContextFactory();
        using var context = contextFactory.CreateContext();
        var repository = new TagOptionRepository(context, _mapper);
        TagId tagId = _fixtures.Tags[0].Id;
        var optionsData = _fixtures.TagOptions.Where(o => o.TagId == tagId).ToList();
        await context.AddRangeAsync(optionsData);
        await context.SaveChangesAsync();
        int offset = 0;
        int pageSize = 10;
        // Act
        var actualOptionsData = (await repository.GetTagOptionsPage(tagId, offset, pageSize))
            .Select(o => o.Map(_mapper))
            .ToList();
        // Assert
        Assert.Equal(actualOptionsData.Count, pageSize);
        optionsData.OrderBy(o => o.Value)
            .Skip(offset)
            .Take(pageSize)
            .Zip(actualOptionsData)
            .ToList()
            .ForEach(pair =>
            {
                Assert.Equal(pair.First.Id, pair.Second.Id);
                Assert.Equal(pair.First.Value, pair.Second.Value);
                Assert.Equal(pair.Second.TagId, tagId);
            });
    }

    [Fact]
    public async Task GroupByTagId_Should_Work()
    {
        // Arrange
        using var contextFactory = new SqliteContextFactory();
        using var context = contextFactory.CreateContext();
        var repository = new TagOptionRepository(context, _mapper);
        var tagIdToOptionDict = _fixtures.TagOptions
            .GroupBy(o => o.TagId)
            .ToDictionary(g => g.Key, g => g.ToList());
        await context.AddRangeAsync(_fixtures.TagOptions);
        await context.SaveChangesAsync();
        // Act
        var actualOptionGrouping = (await repository.GroupByTagIdAsync(_fixtures.TagOptions.Select(o => o.Id)))
            .ToDictionary(pair => pair.Key, pair => pair.Value.Select(o => o.Map(_mapper)).ToList());
        Assert.Equal(tagIdToOptionDict.Count, actualOptionGrouping.Count);
        foreach (TagId tagId in tagIdToOptionDict.Keys)
        {
            List<TagOptionData> expectedOptionsData = tagIdToOptionDict[tagId];
            List<TagOptionData> actualOptionsData = actualOptionGrouping[tagId];
            Assert.Equal(expectedOptionsData.Count, actualOptionsData.Count);
            expectedOptionsData.OrderBy(o => o.Value)
                .Zip(actualOptionsData.OrderBy(o => o.Value))
                .ToList()
                .ForEach(pair =>
                {
                    Assert.Equal(pair.First.Id, pair.Second.Id);
                    Assert.Equal(pair.First.Value, pair.Second.Value);
                    Assert.Equal(pair.Second.TagId, tagId);
                });
        }
    }

    [Fact]
    public async Task FilterCategoryTagOptions_Should_Work()
    {
        // Arrange
        using var contextFactory = new SqliteContextFactory();
        using var context = contextFactory.CreateContext();
        var repository = new TagOptionRepository(context, _mapper);
        var options = _fixtures.TagOptions
            .Where(o => o.Tag.CategoryId == _fixtures.Categories[0].Id)
            .Take(5)
            .ToList();
        List<TagOptionId> ids = options.Select(o => o.Id).ToList();
        await context.AddRangeAsync(_fixtures.TagOptions);
        await context.SaveChangesAsync();
        // Act
        var actualOptionsData = (await repository.FilterCategoryTagOptions(ids, _fixtures.Categories[0].Id))
            .Select(o => o.Map(_mapper))
            .ToList();
        // Assert
        Assert.Equal(ids.Count, actualOptionsData.Count);
        options.OrderBy(o => o.Value)
            .Zip(actualOptionsData.OrderBy(o => o.Value))
            .ToList()
            .ForEach(pair =>
            {
                (TagOptionData expected, TagOptionData actual) = pair;
                Assert.Equal(expected.Id, actual.Id);
                Assert.Equal(expected.Value, actual.Value);
                Assert.Equal(expected.TagId, actual.TagId);
            });
    }

    [Fact]
    public async Task BelongsToCategory_Should_ReturnTrue_If_ItDoes()
    {
        // Arrange
        using var contextFactory = new SqliteContextFactory();
        using var context = contextFactory.CreateContext();
        var repository = new TagOptionRepository(context, _mapper);
        var optionId = _fixtures.TagOptions[0].Id;
        var categoryId = _fixtures.TagOptions[0].Tag.CategoryId;
        await context.AddAsync(_fixtures.TagOptions[0]);
        await context.SaveChangesAsync();
        // Act
        bool result = await repository.BelongsToCategory(optionId, categoryId);
        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task BelongsToCategory_Should_ReturnFalse_IfDoesNot()
    {
        // Arrange
        using var contextFactory = new SqliteContextFactory();
        using var context = contextFactory.CreateContext();
        var repository = new TagOptionRepository(context, _mapper);
        var optionId = _fixtures.TagOptions[0].Id;
        var categoryId = new CategoryId(new Guid());
        await context.AddAsync(_fixtures.TagOptions[0]);
        await context.SaveChangesAsync();
        // Act
        bool result = await repository.BelongsToCategory(optionId, categoryId);
        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task GetProductConfiguration_Should_Work()
    {
        // Arrange
        using var contextFactory = new SqliteContextFactory();
        using var context = contextFactory.CreateContext();
        var repository = new TagOptionRepository(context, _mapper);
        var tagDataMapper = new TagDataMapper();
        ProductData productData = _fixtures.Products[0];
        ProductId productId = productData.Id;
        var productOptions = productData.Options.ToList();
        await context.AddAsync(productData);
        await context.SaveChangesAsync();
        // Act
        List<(TagData Tag, TagOptionData Option)> configuration = (await repository.GetProductConfigurationAsync(productId))
            .Select(tuple => (Tag: tuple.Tag.Map(tagDataMapper), Option: tuple.TagOption.Map(_mapper)))
            .ToList();
        // Assert
        Assert.Equal(productOptions.Count, configuration.Count);
        productOptions.OrderBy(o => o.Value)
            .Zip(configuration.OrderBy(tuple => tuple.Option.Value))
            .ToList()
            .ForEach(pair =>
            {
                (TagOptionData expectedOption, (TagData tagData, TagOptionData actualOption)) = pair;
                Assert.Equal(expectedOption.Id, actualOption.Id);
                Assert.Equal(expectedOption.Value, actualOption.Value);
                Assert.Equal(expectedOption.TagId, actualOption.TagId);
                Assert.Equal(expectedOption.TagId, tagData.Id);
            });
    }

    [Fact]
    public async Task Update_Should_ReturnTrue_If_Success()
    {
        // Arrange
        using var contextFactory = new SqliteContextFactory();
        using var context = contextFactory.CreateContext();
        var repository = new TagOptionRepository(context, _mapper);
        var existingOption = _fixtures.TagOptions[0];
        string updatedValue = "updated_name";
        TagId updatedTagId = _fixtures.Tags[1].Id;
        await context.AddRangeAsync(existingOption, _fixtures.Tags[1]);
        await context.SaveChangesAsync();
        // Act
        bool result = await repository.UpdateAsync(new TagOption(existingOption.Id, updatedValue, updatedTagId));
        await context.SaveChangesAsync();
        // Assert
        Assert.True(result);
        TagOptionData actualOption = await context.Set<TagOptionData>()
            .FirstAsync(o => o.Id == existingOption.Id);
        await context.Entry(actualOption).ReloadAsync();
        Assert.Equal(existingOption.Id, actualOption.Id);
        Assert.Equal(updatedValue, actualOption.Value);
        Assert.Equal(updatedTagId, actualOption.TagId);
    }

    [Fact]
    public async Task Update_Should_ReturnFalse_If_OptionDoesNotExist()
    {
        // Arrange
        using var contextFactory = new SqliteContextFactory();
        using var context = contextFactory.CreateContext();
        var repository = new TagOptionRepository(context, _mapper);
        TagOptionId id = _fixtures.TagOptions[0].Id;
        // Act
        bool result = await repository.UpdateAsync(new TagOption(id, "name", new TagId(new Guid())));
        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task Delete_Should_ReturnTrue_If_Success()
    {
        // Arrange
        using var contextFactory = new SqliteContextFactory();
        using var context = contextFactory.CreateContext();
        var repository = new TagOptionRepository(context, _mapper);
        TagOptionData option = _fixtures.TagOptions[0];
        TagOptionId optionId = option.Id;
        await context.AddAsync(option);
        await context.SaveChangesAsync();
        // Act
        bool result = await repository.DeleteAsync(optionId);
        await context.SaveChangesAsync();
        // Assert
        Assert.True(result);
        TagOptionData? actualOptionData = await context.Set<TagOptionData>().FindAsync(optionId);
        Assert.Null(actualOptionData);
    }

    [Fact]
    public async Task Delete_Should_ReturnFalse_If_IdDoesNotExist()
    {
        // Arrange
        using var contextFactory = new SqliteContextFactory();
        using var context = contextFactory.CreateContext();
        var repository = new TagOptionRepository(context, _mapper);
        var optionId = new TagOptionId(new Guid("9E37267B-534F-4F92-9423-F1082124885E"));
        // Act
        bool result = await repository.DeleteAsync(optionId);
        // Assert
        Assert.False(result);
    }
}
