using Application.Core;
using Application.Exceptions.Categories;
using Application.Files;
using Application.Images;
using Domain.Categories;
using Domain.Files;
using MediatR;

namespace Application.Categories.Commands.DeleteCategory;

internal class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IFileRepository _fileRepository;
    private readonly IFileStorage _fileStorage;
    private readonly ICategoryMapper<CategoryId> _idMapper;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCategoryCommandHandler(
        ICategoryRepository categoryRepository,
        IFileRepository fileRepository,
        IFileStorage fileStorage,
        ICategoryMapper<CategoryId> idMapper,
        IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _fileRepository = fileRepository;
        _fileStorage = fileStorage;
        _idMapper = idMapper;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        if (await _categoryRepository.FindByIdAsync(request.Id) is null)
        {
            throw new CategoryNotFoundException(request.Id);
        }
        List<StoredFile> images = await _fileRepository.FindCategoryFiles(request.Id);
        var parentId = (await _categoryRepository.FindParentAsync(request.Id))?.Map(_idMapper);
        if (parentId is not null)
        {
            await _categoryRepository.UpdateParentIdAsync(request.Id, parentId);
        }
        await _categoryRepository.DeleteAsync(request.Id);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        foreach (StoredFile image in images)
        {
            _fileStorage.DeleteFile(image);
        }
    }
}
