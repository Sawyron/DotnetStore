using Application.Core;
using Application.Exceptions.TagOptions;
using Application.Exceptions.Tags;
using Application.TagOptions;
using Application.TagOptions.Commands.UpdateTagOption;
using Application.Tags;
using Domain.Categories;
using Domain.ProductTypes.Tags.TagOptions;
using Domain.Tags;
using Moq;

namespace Application.UnitTests.TagOptions.Commands.UpdateTagOption;

public class UpdateTagOptionCommandHandlerTest
{
    private readonly Mock<ITagOptionRepository> _optionRepositoryMock = new();
    private readonly Mock<ITagRepository> _tagRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    [Fact]
    public async Task Handle_Should_Work()
    {
        // Arrange
        var tagId = new TagId(new Guid("AA968CEE-ACE3-4A11-81D1-9620E603035D"));
        var optionId = new TagOptionId(new Guid("2413A996-DC10-4881-BB6F-E5A5D023DCAD"));
        _optionRepositoryMock.Setup(repo => repo.FindByIdAsync(optionId))
            .ReturnsAsync(new TagOption(optionId, "value", tagId));
        _tagRepositoryMock.Setup(repo => repo.FindByIdAsync(tagId))
            .ReturnsAsync(new Tag(tagId, "name", new CategoryId(new Guid())));
        var handler = new UpdateTagOptionCommandHandler(
            _optionRepositoryMock.Object,
            _tagRepositoryMock.Object,
            _unitOfWorkMock.Object);
        // Act
        await handler.Handle(new UpdateTagOptionCommand(optionId, "new_value", tagId), default);
        // Assert
        _optionRepositoryMock.Verify(repo => repo.FindByIdAsync(optionId), Times.Once());
        _tagRepositoryMock.Verify(repo => repo.FindByIdAsync(tagId), Times.Once());
        _optionRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<TagOption>()), Times.Once());
        _unitOfWorkMock.Verify(unit => unit.SaveChangesAsync(default));
    }

    [Fact]
    public async Task Handle_Should_Throw_TagOptionNotFoundException_If_OptionDoesNotExist()
    {
        // Act
        var optionId = new TagOptionId(new Guid("2413A996-DC10-4881-BB6F-E5A5D023DCAD"));
        var handler = new UpdateTagOptionCommandHandler(
            _optionRepositoryMock.Object,
            _tagRepositoryMock.Object,
            _unitOfWorkMock.Object);
        // Act & Assert
        await Assert.ThrowsAsync<TagOptionNotFoundException>(async () =>
        {
            await handler.Handle(new UpdateTagOptionCommand(optionId, "value", new TagId(new Guid())), default);
        });
        _optionRepositoryMock.Verify(repo => repo.FindByIdAsync(optionId), Times.Once());
        _optionRepositoryMock.VerifyNoOtherCalls();
        _tagRepositoryMock.VerifyNoOtherCalls();
        _unitOfWorkMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Handle_Should_Throw_TagNotFoundException_If_TagDoesNotExist()
    {
        var tagId = new TagId(new Guid("AA968CEE-ACE3-4A11-81D1-9620E603035D"));
        var optionId = new TagOptionId(new Guid("2413A996-DC10-4881-BB6F-E5A5D023DCAD"));
        var handler = new UpdateTagOptionCommandHandler(
            _optionRepositoryMock.Object,
            _tagRepositoryMock.Object,
            _unitOfWorkMock.Object);
        _optionRepositoryMock.Setup(repo => repo.FindByIdAsync(optionId))
            .ReturnsAsync(new TagOption(optionId, "value", tagId));
        // Act & Assert
        await Assert.ThrowsAsync<TagNotFoundException>(async () =>
        {
            await handler.Handle(new UpdateTagOptionCommand(optionId, "value", tagId), default);
        });
        _optionRepositoryMock.Verify(repo => repo.FindByIdAsync(optionId), Times.Once());
        _tagRepositoryMock.Verify(repo => repo.FindByIdAsync(tagId), Times.Once());
        _optionRepositoryMock.VerifyNoOtherCalls();
        _tagRepositoryMock.VerifyNoOtherCalls();
        _unitOfWorkMock.VerifyNoOtherCalls();
    }
}
