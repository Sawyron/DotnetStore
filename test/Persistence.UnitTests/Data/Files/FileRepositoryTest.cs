using Domain.Categories;
using Domain.Files;
using Microsoft.EntityFrameworkCore;
using Persistence.Data.Files;
using Persistence.Data.Images;

namespace Persistence.UnitTests.Data.Files;

public class FileRepositoryTest
{
    private readonly Fixtures _fixtures = new();
    private readonly FileDataMapper _mapper = new();

    [Fact]
    public async Task Add_Should_Work()
    {
        // Arrange
        using var contextFactory = new SqliteContextFactory();
        using var context = contextFactory.CreateContext();
        var repository = new FileRepository(context, _mapper);
        // Act
        var id = new FileId(new Guid("F4662675-C42B-47E6-9CC7-56756909A852"));
        string path = "path";
        await repository.AddAsync(new StoredFile(id, path));
        await context.SaveChangesAsync();
        // Assert
        FileData actualFile = await context.Set<FileData>().FirstAsync(f => f.Id == id);
        Assert.Equal(id, actualFile.Id);
        Assert.Equal(path, actualFile.Path);
    }

    [Fact]
    public async Task Add_Should_Thorow_InvalidOperationException_When_IdExists()
    {
        // Arrange
        using var contextFactory = new SqliteContextFactory();
        using var context = contextFactory.CreateContext();
        var repository = new FileRepository(context, _mapper);
        FileData fileData = _fixtures.Files[0];
        FileId id = fileData.Id;
        await context.AddAsync(fileData);
        // Act & Assert
        var file = new StoredFile(id, "path");
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await repository.AddAsync(file);
        });
    }

    [Fact]
    public async Task FindById_Should_Work()
    {
        // Arrange
        using var contextFactory = new SqliteContextFactory();
        using var context = contextFactory.CreateContext();
        var repository = new FileRepository(context, _mapper);
        FileData file = _fixtures.Files[0];
        FileId fileId = file.Id;
        await context.AddAsync(file);
        await context.SaveChangesAsync();
        // Act
        FileData? actualFile = (await repository.FindByIdAsync(fileId))?.Map(_mapper);
        Assert.NotNull(actualFile);
        Assert.Equal(fileId, actualFile.Id);
        Assert.Equal(file.Path, actualFile.Path);
    }

    [Fact]
    public async Task FindById_Should_ReturnNull_If_FileDoesNotExist()
    {
        // Arrange
        using var contextFactory = new SqliteContextFactory();
        using var context = contextFactory.CreateContext();
        var repository = new FileRepository(context, _mapper);
        FileId fileId = _fixtures.Files[0].Id;
        // Act
        FileData? actualFile = (await repository.FindByIdAsync(fileId))?.Map(_mapper);
        Assert.Null(actualFile);
    }

    [Fact]
    public async Task FindCategoryFiles_Should_Work()
    {
        // Arrange
        using var contextFactory = new SqliteContextFactory();
        using var context = contextFactory.CreateContext();
        var repository = new FileRepository(context, _mapper);
        CategoryId categoryId = _fixtures.Categories[0].Id;
        var expectedFilesData = _fixtures.Products
            .Where(p => p.CategoryId == categoryId)
            .Select(p => p.Photo)
            .ToList();
        await context.AddRangeAsync(_fixtures.Products);
        await context.SaveChangesAsync();
        // Act
        var actualFilesData = (await repository.FindCategoryFiles(categoryId))
            .Select(f => f.Map(_mapper))
            .ToList();
        Assert.Equal(expectedFilesData.Count, actualFilesData.Count);
        expectedFilesData.OrderBy(f => f.Path)
            .Zip(expectedFilesData.OrderBy(f => f.Path))
            .ToList()
            .ForEach(pair =>
            {
                (FileData expected, FileData actual) = pair;
                Assert.Equal(expected.Id, actual.Id);
                Assert.Equal(expected.Path, actual.Path);
            });
    }

    [Fact]
    public async Task Delete_Should_ReturnTrue_If_Success()
    {
        // Arrange
        using var contextFactory = new SqliteContextFactory();
        using var context = contextFactory.CreateContext();
        var repository = new FileRepository(context, _mapper);
        FileId fileId = _fixtures.Files[0].Id;
        await context.AddAsync(_fixtures.Files[0]);
        await context.SaveChangesAsync();
        // Act
        bool result = await repository.DeleteAsync(fileId);
        await context.SaveChangesAsync();
        // Assert
        Assert.True(result);
        FileData? actualFile = await context.Set<FileData>()
            .FindAsync(fileId);
        Assert.Null(actualFile);
    }

    [Fact]
    public async Task Delete_Should_ReturnFalse_If_FileDoesNotExist()
    {
        // Arrange
        using var contextFactory = new SqliteContextFactory();
        using var context = contextFactory.CreateContext();
        var repository = new FileRepository(context, _mapper);
        FileId fileId = _fixtures.Files[0].Id;
        // Act
        bool result = await repository.DeleteAsync(fileId);
        // Assert
        Assert.False(result);
    }
}
