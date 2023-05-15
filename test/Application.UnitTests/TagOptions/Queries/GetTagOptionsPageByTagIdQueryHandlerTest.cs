using Application.TagOptions;
using Application.TagOptions.Queries.GetPageByTagId;
using Domain.ProductTypes.Tags.TagOptions;
using Domain.Tags;
using Moq;

namespace Application.UnitTests.TagOptions.Queries;

public class GetTagOptionsPageByTagIdQueryHandlerTest
{
    private readonly Mock<ITagOptionRepository> _repositoryMock;
    private readonly TagOptionResponseMapper _mapper;

    public GetTagOptionsPageByTagIdQueryHandlerTest()
    {
        _repositoryMock = new();
        _mapper = new();
    }

    [Fact]
    public async Task Handle_Should_ReturnFullPage()
    {
        // Arrange
        var tagId = new TagId(new Guid("FD07E6BE-E2B9-4B78-A61D-4CBDF6DA6F37"));
        int offset = 0;
        int pageSize = 10;
        var options = Enumerable.Range(1, pageSize)
            .Select(i => new TagOption(
                new TagOptionId(new Guid($"0A05EE9F-F371-4670-A6B6-7FF8897CE2E{i % 10}")),
                $"tag_option_name_{i}",
                tagId))
            .ToList();
        _repositoryMock.Setup(repo => repo.GetTagOptionsPage(tagId, offset, pageSize))
            .ReturnsAsync(options);
        var handler = new GetTagOptinsPageByTagIdQueryHandler(_repositoryMock.Object, _mapper);
        // Act
        var response = await handler.Handle(new GetTagOptionsPageByTagIdQuery(offset, pageSize, tagId), default);
        // Assert
        options.Select(o => o.Map(_mapper))
            .Zip(response)
            .ToList()
            .ForEach(pair => Assert.Equal(pair.First, pair.Second));
        _repositoryMock.Verify(repo => repo.GetTagOptionsPage(tagId, offset, pageSize), Times.Once());
    }
}
