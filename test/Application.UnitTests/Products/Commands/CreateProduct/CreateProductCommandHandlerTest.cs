using Application.Core;
using Application.Exceptions.Products;
using Application.Files;
using Application.Files.Commands.CreateFile;
using Application.Products;
using Application.Products.Commands.CreateProduct;
using Application.TagOptions;
using Domain.Categories;
using Domain.Files;
using Domain.Products;
using Domain.ProductTypes.Tags.TagOptions;
using Domain.Tags;
using Moq;

namespace Application.UnitTests.Products.Commands.CreateProduct;

public class CreateProductCommandHandlerTest
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<ITagOptionRepository> _tagOptionRepositoryMock;
    private readonly TagOptionTupleMapper _tagOptionTupleMapper;
    private readonly Mock<IFileStorage> _fileStorageMock;
    private readonly Mock<IUnitOfWork> _unifOfWorkMock;

    public CreateProductCommandHandlerTest()
    {
        _productRepositoryMock = new();
        _tagOptionRepositoryMock = new();
        _tagOptionTupleMapper = new();
        _fileStorageMock = new();
        _unifOfWorkMock = new();
    }

    [Fact]
    public async Task Handle_Should_Work()
    {
        // Arrane
        var categoryId = new CategoryId(new Guid());
        List<TagId> tagIds = Enumerable.Range(1, 5)
            .Select(i => new TagId(new Guid($"0022B071-E0E9-4824-81B8-4714C572A1F{i % 10}")))
            .ToList();
        List<Tag> tags = tagIds
            .Select((id, i) => new Tag(
                id,
                $"tag_name_{i}",
                categoryId))
            .ToList();
        Dictionary<TagId, List<TagOption>> tagIdToOptionDict = tagIds
            .ToDictionary(
                id => id,
                id => new List<TagOption>()
                {
                    new TagOption(
                        new TagOptionId(new Guid()),
                        $"tag_option_name_{id.Value}",
                        id)
                });
        var tagOptionIds = tagIdToOptionDict.Values
            .SelectMany(options => options)
            .Select(o => o.Map(_tagOptionTupleMapper).Item1)
            .ToList();
        _tagOptionRepositoryMock.Setup(repo => repo.FilterCategoryTagOptions(tagOptionIds, categoryId))
            .ReturnsAsync(tagIdToOptionDict.Values.SelectMany(options => options));
        _tagOptionRepositoryMock.Setup(repo => repo.GroupByTagIdAsync(tagOptionIds))
            .ReturnsAsync(tagIdToOptionDict);
        var fileContent = new byte[] { 1, 2, 3, 4, 5 };
        var fileExtension = ".png";
        _fileStorageMock.Setup(s => s.SaveFileAsync(fileContent, fileExtension))
            .ReturnsAsync(new StoredFile(new FileId(new Guid()), string.Empty));
        string productName = "productName";
        int productPrice = 100;
        string productDescription = "description";
        var handler = new CreateProductCommandHandler(
            _productRepositoryMock.Object,
            _tagOptionRepositoryMock.Object,
            _tagOptionTupleMapper,
            _fileStorageMock.Object,
            _unifOfWorkMock.Object);
        // Act
        await handler.Handle(new CreateProductCommand(
            productName,
            productPrice,
            productDescription,
            tagOptionIds,
            new CreateFileCommand(fileContent, fileExtension),
            categoryId), default);
        // Assert
        _tagOptionRepositoryMock.Verify(repo => repo.GroupByTagIdAsync(tagOptionIds), Times.Once());
        _tagOptionRepositoryMock.Verify(repo => repo.FilterCategoryTagOptions(tagOptionIds, categoryId), Times.Once());
        _fileStorageMock.Verify(s => s.SaveFileAsync(fileContent, fileExtension), Times.Once());
        _productRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Product>()), Times.Once());
        _unifOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once());
    }

    [Fact]
    public async Task Handle_Should_Throw_TagOptionIdsTagCollisionException_If_SomeOptionIdsBelongToTheSameCategory()
    {
        // Arrane
        var categoryId = new CategoryId(new Guid());
        List<TagId> tagIds = Enumerable.Range(1, 5)
            .Select(i => new TagId(new Guid($"0022B071-E0E9-4824-81B8-4714C572A1F{i % 10}")))
            .ToList();
        List<Tag> tags = tagIds
            .Select((id, i) => new Tag(
                id,
                $"tag_name_{i}",
                categoryId))
            .ToList();
        Dictionary<TagId, List<TagOption>> tagIdToOptionDict = tagIds
            .ToDictionary(
                id => id,
                id => new List<TagOption>()
                {
                    new TagOption(
                        new TagOptionId(new Guid()),
                        $"tag_option_name_{id.Value}",
                        id),
                    new TagOption(
                        new TagOptionId(new Guid()),
                        $"tag_option_name_{id.Value}",
                        id)
                });
        var tagOptionIds = tagIdToOptionDict.Values
            .SelectMany(options => options)
            .Select(o => o.Map(_tagOptionTupleMapper).Item1)
            .ToList();
        _tagOptionRepositoryMock.Setup(repo => repo.GroupByTagIdAsync(tagOptionIds))
            .ReturnsAsync(tagIdToOptionDict);
        var handler = new CreateProductCommandHandler(
            _productRepositoryMock.Object,
            _tagOptionRepositoryMock.Object,
            _tagOptionTupleMapper,
            _fileStorageMock.Object,
            _unifOfWorkMock.Object);
        // Act & Assert
        await Assert.ThrowsAsync<TagOptionIdsTagCollisionException>(async () =>
        {
            await handler.Handle(new CreateProductCommand(
            "productName",
            100,
            "productDescription",
            tagOptionIds,
            new CreateFileCommand(new byte[] { 1 }, "fileExtension"),
            categoryId), default);
        });
        _tagOptionRepositoryMock.Verify(repo => repo.GroupByTagIdAsync(tagOptionIds), Times.Once());
        _productRepositoryMock.VerifyNoOtherCalls();
        _unifOfWorkMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Handle_Should_Throw_TagOptionIdsCategoryConflictException_If_OptionIdsAreWrong()
    {
        // Arrane
        var categoryId = new CategoryId(new Guid());
        List<TagId> tagIds = Enumerable.Range(1, 5)
            .Select(i => new TagId(new Guid($"0022B071-E0E9-4824-81B8-4714C572A1F{i % 10}")))
            .ToList();
        List<Tag> tags = tagIds
            .Select((id, i) => new Tag(
                id,
                $"tag_name_{i}",
                categoryId))
            .ToList();
        Dictionary<TagId, List<TagOption>> tagIdToOptionDict = tagIds
            .ToDictionary(
                id => id,
                id => new List<TagOption>()
                {
                    new TagOption(
                        new TagOptionId(new Guid()),
                        $"tag_option_name_{id.Value}",
                        id)
                });
        var tagOptionIdsToForCommand = Enumerable.Range(1, 5)
            .Select(i => new TagOptionId(new Guid($"6524407B-237F-4BEA-B4AC-8874C0F3B52{i}")))
            .ToList();
        _tagOptionRepositoryMock.Setup(repo => repo.FilterCategoryTagOptions(tagOptionIdsToForCommand, categoryId))
            .ReturnsAsync(Array.Empty<TagOption>());
        _tagOptionRepositoryMock.Setup(repo => repo.GroupByTagIdAsync(tagOptionIdsToForCommand))
            .ReturnsAsync(tagIdToOptionDict);
        var handler = new CreateProductCommandHandler(
            _productRepositoryMock.Object,
            _tagOptionRepositoryMock.Object,
            _tagOptionTupleMapper,
            _fileStorageMock.Object,
            _unifOfWorkMock.Object);
        // Act & Assert
        await Assert.ThrowsAsync<TagOptionIdsCategoryConflictException>(async () =>
        {
            await handler.Handle(new CreateProductCommand(
                "productName",
                100,
                "productDescription",
                tagOptionIdsToForCommand,
                new CreateFileCommand(new byte[] { 1 }, "fileExtension"),
                categoryId), default);
        });
        _tagOptionRepositoryMock.Verify(repo => repo.GroupByTagIdAsync(tagOptionIdsToForCommand), Times.Once());
        _tagOptionRepositoryMock.Verify(
            repo => repo.FilterCategoryTagOptions(tagOptionIdsToForCommand, categoryId),
            Times.Once());
        _fileStorageMock.VerifyNoOtherCalls();
        _productRepositoryMock.VerifyNoOtherCalls();
        _unifOfWorkMock.VerifyNoOtherCalls();
    }
}
