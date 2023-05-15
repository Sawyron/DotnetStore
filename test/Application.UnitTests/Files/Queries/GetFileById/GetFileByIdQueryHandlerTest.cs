using Application.Exceptions.Files;
using Application.Files;
using Application.Files.Queries.GetFileById;
using Application.Images;
using Domain.Files;
using Moq;

namespace Application.UnitTests.Files.Queries.GetFileById;

public class GetFileByIdQueryHandlerTest
{
    private readonly Mock<IFileStorage> _storageMock;
    private readonly Mock<IFileRepository> _repositoryMock;

    public GetFileByIdQueryHandlerTest()
    {
        _storageMock = new();
        _repositoryMock = new();
    }


    [Fact]
    public async Task Hanlle_Shold_ReturnFile()
    {
        // Arrrange
        var id = new FileId(new Guid("4902CB00-7AB7-4E76-A86E-E2D98A18BDCD"));
        var file = new StoredFile(id, "path");
        byte[] content = new byte[] { 1, 2, 3, 4, 5 };
        string extension = ".jpeg";
        _repositoryMock.Setup(repo => repo.FindByIdAsync(id)).ReturnsAsync(file);
        _storageMock.Setup(s => s.ReadFileAsync(file)).ReturnsAsync((content, extension));
        var handler = new GetFileByIdQueryHandler(_storageMock.Object, _repositoryMock.Object);
        // Act
        var response = await handler.Handle(new GetFileByIdQuery(id), default);
        // Assert
        Assert.Equal(content, response.Content);
        Assert.Equal(extension, response.Extension);
        _repositoryMock.Verify(repo => repo.FindByIdAsync(id), Times.Once());
        _storageMock.Verify(s => s.ReadFileAsync(file), Times.Once());
    }

    [Fact]
    public async Task Hanlle_Shold_Thorow_StoredFileNotFoundException_When_File_DoesNotExists()
    {
        // Arrrange
        var id = new FileId(new Guid("4902CB00-7AB7-4E76-A86E-E2D98A18BDCD"));
        _repositoryMock.Setup(repo => repo.FindByIdAsync(id)).ReturnsAsync((StoredFile)null!);
        var handler = new GetFileByIdQueryHandler(_storageMock.Object, _repositoryMock.Object);
        // Act
        await Assert.ThrowsAsync<StoredFileNotFoundException>(async () =>
        {
            await handler.Handle(new GetFileByIdQuery(id), default);
        });
    }

}
