using Domain.Categories;
using Domain.Files;
using Domain.Products;
using Domain.ProductTypes.Tags.TagOptions;
using Microsoft.EntityFrameworkCore;
using Persistence.Data.Images;
using Persistence.Data.Products;
using Persistence.Data.TagOptions;

namespace Persistence.UnitTests.Data.Products;

public class ProductRepositoryTest
{
    private readonly Fixtures _fixtures = new();
    private readonly ProductDataMapper _mapper = new(new FileDataMapper());

    [Fact]
    public async Task Add_Should_Work()
    {
        // Arrange
        using var contextFactory = new SqliteContextFactory();
        using var context = contextFactory.CreateContext();
        var repository = new ProductRepository(context, _mapper);
        List<TagOptionId> optionIds = _fixtures.Products[0]
            .Options
            .Select(o => o.Id)
            .ToList();
        CategoryId categoryId = _fixtures.Categories[0].Id;
        var photo = new StoredFile(new FileId(new Guid("22C47165-10EF-4E4A-AC07-4CE6F4A7CF2D")), "path");
        FileData photoData = photo.Map(new FileDataMapper());
        string name = "product_name";
        int price = 1000000;
        string description = "desc";
        var productId = new ProductId(new Guid("308DCF97-A859-4649-AF36-57F2872EBB45"));
        var product = new Product(
            productId,
            name,
            price,
            description,
            photo,
            optionIds,
            categoryId);
        await context.AddRangeAsync(_fixtures.TagOptions);
        await context.SaveChangesAsync();
        // Act
        await repository.AddAsync(product);
        await context.SaveChangesAsync();
        // Asseert
        ProductData actualProductData = (await context.Set<ProductData>()
            .Include(p => p.Photo)
            .FirstAsync(p => p.Id == productId));
        Assert.Equal(productId, actualProductData.Id);
        Assert.Equal(name, actualProductData.Name);
        Assert.Equal(price, actualProductData.Price);
        Assert.Equal(description, actualProductData.Description);
        Assert.Equal(photoData.Id, actualProductData.Photo.Id);
        Assert.Equal(photoData.Path, actualProductData.Photo.Path);
    }

    [Fact]
    public async Task Add_Should_Throw_If_IdExists()
    {
        // Arrange
        using var contextFactory = new SqliteContextFactory();
        using var context = contextFactory.CreateContext();
        var repository = new ProductRepository(context, _mapper);
        var productData = _fixtures.Products[0];
        var productId = productData.Id;
        var product = new Product(
            productId,
            "name",
            100,
            "desc",
            new StoredFile(new FileId(new Guid("F44373D9-E842-4846-8543-5603CDD387DA")), "path"),
            Array.Empty<TagOptionId>(),
            new CategoryId(new Guid("D43CFF5C-C7F0-40C5-B2F1-ACF412A30166")));
        await context.AddAsync(productData);
        await context.SaveChangesAsync();
        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await repository.AddAsync(product);
        });
    }

    [Fact]
    public async Task Add_Should_Throw_DbUpdateException_If_CategoryIdDoesNotExist()
    {
        // Arrange
        using var contextFactory = new SqliteContextFactory();
        using var context = contextFactory.CreateContext();
        var repository = new ProductRepository(context, _mapper);
        var productId = new ProductId(new Guid("2695BA8E-7B86-45D4-8593-F150DEA1E486"));
        var product = new Product(
            productId,
            "name",
            100,
            "desc",
            new StoredFile(new FileId(new Guid("F44373D9-E842-4846-8543-5603CDD387DA")), "path"),
            Array.Empty<TagOptionId>(),
            new CategoryId(new Guid("D43CFF5C-C7F0-40C5-B2F1-ACF412A30166")));
        await context.SaveChangesAsync();
        // Act & Assert
        await Assert.ThrowsAsync<DbUpdateException>(async () =>
        {
            await repository.AddAsync(product);
            await context.SaveChangesAsync();
        });
    }

    [Fact]
    public async Task FindById_Should_Work()
    {
        // Arrange
        using var contextFactory = new SqliteContextFactory();
        using var context = contextFactory.CreateContext();
        var repository = new ProductRepository(context, _mapper);
        var productData = _fixtures.Products[0];
        var productId = productData.Id;
        await context.AddAsync(productData);
        await context.SaveChangesAsync();
        // Act
        ProductData? actualProductData = (await repository.FindByIdAsync(productId))?.Map(_mapper);
        Assert.NotNull(actualProductData);
        Assert.Equal(productData.Id, actualProductData.Id);
        Assert.Equal(productData.Name, actualProductData.Name);
        Assert.Equal(productData.Price, actualProductData.Price);
        Assert.Equal(productData.Description, actualProductData.Description);
        Assert.Equal(productData.Photo.Id, actualProductData.Photo.Id);
        Assert.Equal(productData.Photo.Path, actualProductData.Photo.Path);
        Assert.Equal(productData.Options.Count, actualProductData.OptionIds.Count());
        foreach (TagOptionId optionId in productData.Options.Select(o => o.Id))
        {
            Assert.Contains(optionId, actualProductData.OptionIds);
        }
    }

    [Fact]
    public async Task FindById_Should_ReturnNull_If_IdDoesNotExist()
    {
        // Arrange
        using var contextFactory = new SqliteContextFactory();
        using var context = contextFactory.CreateContext();
        var repository = new ProductRepository(context, _mapper);
        var productId = _fixtures.Products[0].Id;
        // Act
        Product? actualProduct = await repository.FindByIdAsync(productId);
        Assert.Null(actualProduct);
    }

    [Fact]
    public async Task GetCategoryProductsPage_Should_If_Work()
    {
        // Arrange
        using var contextFactory = new SqliteContextFactory();
        using var context = contextFactory.CreateContext();
        var repository = new ProductRepository(context, _mapper);
        await context.AddRangeAsync(_fixtures.Products);
        await context.SaveChangesAsync();
        int offset = 0;
        int pageSize = 10;
        CategoryId categoryId = _fixtures.Categories[0].Id;
        // Act
        var actualProductsDataPage = (await repository.GetCategoryProductsPageAsync(offset, pageSize, categoryId))
            .Select(p => p.Map(_mapper))
            .ToList();
        Assert.Equal(actualProductsDataPage.Count, pageSize);
        _fixtures.Products
            .Where(p => p.CategoryId == categoryId)
            .OrderBy(p => p.Name)
            .Skip(offset)
            .Take(pageSize)
            .Zip(actualProductsDataPage)
            .ToList()
            .ForEach(pair =>
            {
                (ProductData expectedProductData, ProductData actualProductData) = pair;
                Assert.Equal(expectedProductData.Id, actualProductData.Id);
                Assert.Equal(expectedProductData.Name, actualProductData.Name);
                Assert.Equal(expectedProductData.Description, actualProductData.Description);
                Assert.Equal(expectedProductData.Price, actualProductData.Price);
                Assert.Equal(categoryId, actualProductData.CategoryId);
                foreach (TagOptionId optionId in expectedProductData.Options.Select(o => o.Id))
                {
                    Assert.Contains(optionId, actualProductData.OptionIds);
                }
            });
    }

    [Fact]
    public async Task Update_Should_ReturnTrue_If_Success()
    {
        // Arrange
        using var contextFactory = new SqliteContextFactory();
        using var context = contextFactory.CreateContext();
        var repository = new ProductRepository(context, _mapper);
        var productData = _fixtures.Products[0];
        var productId = productData.Id;
        string updatedName = "new_name";
        int updatedPrice = 20000;
        string updatedDescription = "new_decs";
        CategoryId updatedCategoryId = _fixtures.Categories[1].Id;
        var updatedProduct = new Product(
            productId,
            updatedName,
            updatedPrice,
            updatedDescription,
            new StoredFile(new FileId(new Guid("26BFB252-E62A-44B8-95F8-CFFC9E30BE8F")), "path"),
            Array.Empty<TagOptionId>(),
            updatedCategoryId);
        await context.AddAsync(productData);
        await context.SaveChangesAsync();
        // Act
        bool result = await repository.UpdateAsync(updatedProduct);
        await context.SaveChangesAsync();
        // Assert
        Assert.True(result);
        ProductData? actualProductData = await context.Set<ProductData>().FindAsync(productId);
        Assert.NotNull(actualProductData);
        await context.Entry(actualProductData).ReloadAsync();
        Assert.Equal(productId, actualProductData.Id);
        Assert.Equal(updatedName, actualProductData.Name);
        Assert.Equal(updatedPrice, actualProductData.Price);
        Assert.Equal(updatedDescription, actualProductData.Description);
        Assert.Equal(updatedCategoryId, actualProductData.CategoryId);
    }

    [Fact]
    public async Task Update_Should_ReturnFalse_If_IdDoesNotExist()
    {
        // Arrange
        using var contextFactory = new SqliteContextFactory();
        using var context = contextFactory.CreateContext();
        var repository = new ProductRepository(context, _mapper);
        var productId = new ProductId(new Guid("FBF294DA-780A-4138-9760-4299F6EB58BA"));
        var product = new Product(
            productId,
            "name",
            100,
            "desc",
            new StoredFile(new FileId(new Guid("F44373D9-E842-4846-8543-5603CDD387DA")), "path"),
            Array.Empty<TagOptionId>(),
            new CategoryId(new Guid("D43CFF5C-C7F0-40C5-B2F1-ACF412A30166")));
        // Act
        bool result = await repository.UpdateAsync(product);
        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task UpdatePhoto_Should_Work()
    {
        // Arrange
        using var contextFactory = new SqliteContextFactory();
        using var context = contextFactory.CreateContext();
        var repository = new ProductRepository(context, _mapper);
        var productId = _fixtures.Products[0].Id;
        var updatedFileId = _fixtures.Files[1].Id;
        await context.AddRangeAsync(_fixtures.Products[0], _fixtures.Files[1]);
        await context.SaveChangesAsync();
        // Act
        bool result = await repository.UpdatePhoto(productId, updatedFileId);
        await context.SaveChangesAsync();
        // Assert
        Assert.True(result);
        ProductData actualProductData = await context.Set<ProductData>()
            .FirstAsync(p => p.Id == productId);
        await context.Entry(actualProductData).ReloadAsync();
        Assert.Equal(updatedFileId, actualProductData.PhotoId);
    }

    [Fact]
    public async Task UpdatePhoto_Should_Return_False_IF_ProductDoes_NotExist()
    {
        // Arrange
        using var contextFactory = new SqliteContextFactory();
        using var context = contextFactory.CreateContext();
        var repository = new ProductRepository(context, _mapper);
        var productId = _fixtures.Products[0].Id;
        var updatedFileId = _fixtures.Files[1].Id;
        await context.AddRangeAsync(_fixtures.Files[1]);
        await context.SaveChangesAsync();
        // Act
        bool result = await repository.UpdatePhoto(productId, updatedFileId);
        await context.SaveChangesAsync();
        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task UpdateConfiguration_Should_ReturnTrue_If_Success()
    {
        // Arrange
        using var contextFactory = new SqliteContextFactory();
        using var context = contextFactory.CreateContext();
        var repository = new ProductRepository(context, _mapper);
        ProductData product = _fixtures.Products[0];
        List<TagOptionData> existingOptions = product.Options.Take(5).ToList();
        List<TagOptionData> updatedOptions = product.Options.Skip(5).Take(5).ToList();
        await context.AddRangeAsync(product.Options);
        product.Options = existingOptions;
        await context.AddAsync(product);
        await context.SaveChangesAsync();
        // Act
        bool result = await repository.UpdateConfigurationAsync(product.Id, updatedOptions.Select(o => o.Id));
        await context.SaveChangesAsync();
        // Assert
        ProductData actualProduct = await context.Set<ProductData>()
            .Include(p => p.Options)
            .FirstAsync(p => p.Id == product.Id);
        List<TagOptionData> actualOptions = actualProduct.Options.ToList();
        Assert.Equal(updatedOptions.Count, actualOptions.Count);
        updatedOptions.OrderBy(o => o.Value)
            .Zip(actualOptions.OrderBy(o => o.Value))
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
    public async Task UpdateConfiguration_Should_ReturnFalse_If_ProductDoesNotExist()
    {
        // Arrange
        using var contextFactory = new SqliteContextFactory();
        using var context = contextFactory.CreateContext();
        var repository = new ProductRepository(context, _mapper);
        ProductId productId = _fixtures.Products[0].Id;
        // Act
        bool result = await repository.UpdateConfigurationAsync(productId, Array.Empty<TagOptionId>());
        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task Delete_Should_ReturnTrue_If_Success()
    {
        // Arrange
        using var contextFactory = new SqliteContextFactory();
        using var context = contextFactory.CreateContext();
        var repository = new ProductRepository(context, _mapper);
        ProductData productData = _fixtures.Products[0];
        ProductId productId = productData.Id;
        await context.AddAsync(productData);
        await context.SaveChangesAsync();
        // Act
        bool result = await repository.DeleteAsync(productId);
        await context.SaveChangesAsync();
        // Assert
        Assert.True(result);
        ProductData? actualProductData = await context.Set<ProductData>().FindAsync(productId);
        Assert.Null(actualProductData);
    }

    [Fact]
    public async Task Delete_Should_ReturnFalse_If_IdDoesNotExist()
    {
        // Arrange
        using var contextFactory = new SqliteContextFactory();
        using var context = contextFactory.CreateContext();
        var repository = new ProductRepository(context, _mapper);
        var productId = new ProductId(new Guid("FBF294DA-780A-4138-9760-4299F6EB58BA"));
        // Act
        bool result = await repository.DeleteAsync(productId);
        // Assert
        Assert.False(result);
    }
}
