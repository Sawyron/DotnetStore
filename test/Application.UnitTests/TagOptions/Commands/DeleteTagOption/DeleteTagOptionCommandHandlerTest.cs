using Application.Core;
using Application.Exceptions.TagOptions;
using Application.TagOptions;
using Application.TagOptions.Commands.DeleteTagOption;
using Domain.ProductTypes.Tags.TagOptions;
using Moq;

namespace Application.UnitTests.TagOptions.Commands.DeleteTagOption;

public class DeleteTagOptionCommandHandlerTest
{
    private readonly Mock<ITagOptionRepository> _repositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public DeleteTagOptionCommandHandlerTest()
    {
        _repositoryMock = new();
        _unitOfWorkMock = new();
    }

    [Fact]
    public async Task Handle_Should_Work()
    {
        // Arrange
        var id = new TagOptionId(new Guid("C4EBCFC8-9A87-4052-AFA1-3848D35621C1"));
        _repositoryMock.Setup(repo => repo.DeleteAsync(id))
            .ReturnsAsync(true);
        var handler = new DeleteTagOptionCommandHandler(_repositoryMock.Object, _unitOfWorkMock.Object);
        // Act
        await handler.Handle(new DeleteTagOptionCommand(id), default);
        // Assert
        _repositoryMock.Verify(repo => repo.DeleteAsync(id), Times.Once());
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once());
    }

    [Fact]
    public async Task Handle_Should_Throw_TagOptionNotFoundException_When_OptionExists()
    {
        // Arrange
        var id = new TagOptionId(new Guid("C4EBCFC8-9A87-4052-AFA1-3848D35621C1"));
        _repositoryMock.Setup(repo => repo.DeleteAsync(id))
            .ReturnsAsync(false);
        var handler = new DeleteTagOptionCommandHandler(_repositoryMock.Object, _unitOfWorkMock.Object);
        // Act & Assert
        await Assert.ThrowsAsync<TagOptionNotFoundException>(async () =>
        {
            await handler.Handle(new DeleteTagOptionCommand(id), default);
        });
        _repositoryMock.Verify(repo => repo.DeleteAsync(id), Times.Once());
        _unitOfWorkMock.VerifyNoOtherCalls();
    }
}
