using Application.Core;
using Application.Exceptions.Tags;
using Application.TagOptions;
using Application.TagOptions.Commands.CreateTagOption;
using Application.Tags;
using Domain.Categories;
using Domain.ProductTypes.Details;
using Domain.ProductTypes.Tags.TagOptions;
using Moq;

namespace Application.UnitTests.TagOptions.Commands.CreateTagOption;

public class CreateTagOptionCommandHandlerTest
{
    private readonly Mock<ITagOptionRepository> _tagOptionRepositoryMock;
    private readonly Mock<ITagRepository> _tagRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public CreateTagOptionCommandHandlerTest()
    {
        _tagOptionRepositoryMock = new();
        _tagRepositoryMock = new();
        _unitOfWorkMock = new();
    }

    [Fact]
    public async Task Hanlde_Should_Work()
    {
        // Arrage
        var tagId = new TagId(new Guid("0BD1D496-D272-4E23-9289-6ED84F950CFF"));
        _tagRepositoryMock.Setup(repo => repo.FindByIdAsync(tagId))
            .ReturnsAsync(new Tag(
                tagId, string.Empty,
                new CategoryId(new Guid("2AC95098-A7B9-40E1-8E21-5A06CAC077A7"))));
        var hanlder = new CreateTagOptonCommandHandler(
            _tagOptionRepositoryMock.Object,
            _tagRepositoryMock.Object,
            _unitOfWorkMock.Object);
        // Act
        await hanlder.Handle(new CreateTagOptionCommand(string.Empty, tagId), default);
        // Assert
        _tagRepositoryMock.Verify(repo => repo.FindByIdAsync(tagId), Times.Once());
        _tagOptionRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<TagOption>()), Times.Once());
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once());
    }

    [Fact]
    public async Task Hanlde_Should_Thorw_TagNotFoundException_WhenTagDoesNotExist()
    {
        // Arrage
        var tagId = new TagId(new Guid("0BD1D496-D272-4E23-9289-6ED84F950CFF"));
        _tagRepositoryMock.Setup(repo => repo.FindByIdAsync(tagId))
            .ReturnsAsync((Tag)null!);
        var hanlder = new CreateTagOptonCommandHandler(
            _tagOptionRepositoryMock.Object,
            _tagRepositoryMock.Object,
            _unitOfWorkMock.Object);
        // Act & Assert
        await Assert.ThrowsAsync<TagNotFoundException>(async () =>
        {
            await hanlder.Handle(new CreateTagOptionCommand(string.Empty, tagId), default);
        });
        _tagRepositoryMock.Verify(repo => repo.FindByIdAsync(tagId), Times.Once());
        _tagOptionRepositoryMock.VerifyNoOtherCalls();
        _unitOfWorkMock.VerifyNoOtherCalls();
    }
}
