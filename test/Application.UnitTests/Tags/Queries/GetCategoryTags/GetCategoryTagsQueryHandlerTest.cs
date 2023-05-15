using Application.Tags;
using Application.Tags.Queries.GetCategoryTags;
using Domain.Categories;
using Domain.Tags;
using Moq;

namespace Application.UnitTests.Tags.Queries.GetCategoryTags;

public class GetCategoryTagsQueryHandlerTest
{
    private readonly Mock<ITagRepository> _repositoryMock;
    private readonly TagResponseMapper _mapper;

    public GetCategoryTagsQueryHandlerTest()
    {
        _repositoryMock = new();
        _mapper = new();
    }

    [Fact]
    public async Task Handle_Should_Work()
    {
        // Arrange
        var categoryId = new CategoryId(new Guid("B56A2C58-3E18-4EA6-BCDB-59A46178553F"));
        var tags = Enumerable.Range(1, 5)
            .Select(i => new Tag(
                new TagId(new Guid($"20CC9C3E-62AE-47A4-86B6-5685AB3219F{i % 10}")),
                $"tag_name_{i}",
                categoryId))
            .ToList();
        _repositoryMock.Setup(repo => repo.FindCategoryTags(categoryId))
            .ReturnsAsync(tags);
        var handler = new GetCategoryTagsQueryHandler(_repositoryMock.Object, _mapper);
        // Act
        var response = await handler.Handle(new GetCategoryTagsQuery(categoryId), default);
        // Assert
        response.
            Zip(tags.Select(t => t.Map(_mapper)))
            .ToList()
            .ForEach(pair => Assert.Equal(pair.First, pair.Second));
        _repositoryMock.Verify(repo => repo.FindCategoryTags(categoryId), Times.Once());
    }
}
