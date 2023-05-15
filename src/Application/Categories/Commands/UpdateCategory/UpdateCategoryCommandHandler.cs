using Application.Core;
using Application.Exceptions.Categories;
using Domain.Categories;
using MediatR;

namespace Application.Categories.Commands.UpdateCategory;

internal class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCategoryCommandHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        if (await _categoryRepository.FindByIdAsync(request.Id) is null)
        {
            throw new CategoryNotFoundException(request.Id);
        }
        if (request.ParentId is CategoryId parentId && await _categoryRepository.FindByIdAsync(parentId) is null)
        {
            throw new CategoryNotFoundException(parentId);
        }
        var category = new Category(request.Id, request.Name);
        await _categoryRepository.UpdateAsync(category, request.ParentId);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
