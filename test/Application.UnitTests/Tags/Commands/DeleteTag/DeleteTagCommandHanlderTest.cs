using Application.Core;
using Application.Exceptions.Tags;
using Application.Tags;
using Application.Tags.Commands.DeleteTag;
using Domain.ProductTypes.Details;
using Moq;

namespace Application.UnitTests.Tags.Commands.DeleteTag;

public class DeleteTagCommandHanlderTest
{
    private readonly Mock<ITagRepository> _repositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public DeleteTagCommandHanlderTest()
    {
        _repositoryMock = new();
        _unitOfWorkMock = new();
    }

    [Fact]
    public async Task Handle_Should_Work()
    {
        // Arrange
        var id = new TagId(new Guid("04057CBD-9FC1-4D82-8C01-D5FD83E7D077"));
        _repositoryMock.Setup(repo => repo.DeleteAsync(id))
            .ReturnsAsync(true);
        var handler = new DeleteTagCommandHandler(_repositoryMock.Object, _unitOfWorkMock.Object);
        // Act
        await handler.Handle(new DeleteTagCommand(id), default);
        // Assert
        _repositoryMock.Verify(repo => repo.DeleteAsync(id), Times.Once());
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once());
    }

    [Fact]
    public async Task Handle_Should_Throw_TagNotFoundException_When_TagWithIdDoesNotExist()
    {
        // Arrange
        var id = new TagId(new Guid("04057CBD-9FC1-4D82-8C01-D5FD83E7D077"));
        _repositoryMock.Setup(repo => repo.DeleteAsync(id))
            .ReturnsAsync(false);
        var handler = new DeleteTagCommandHandler(_repositoryMock.Object, _unitOfWorkMock.Object);
        // Act & Assert
        await Assert.ThrowsAsync<TagNotFoundException>(async () =>
        {
            await handler.Handle(new DeleteTagCommand(id), default);
        });
        _repositoryMock.Verify(repo => repo.DeleteAsync(id), Times.Once());
        _unitOfWorkMock.VerifyNoOtherCalls();
    }
}
