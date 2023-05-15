using Application.Core;
using Application.Exceptions.Categories;
using Domain.Categories;
using MediatR;

namespace Application.Categories.Commands.CreateCategory;

internal class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CategoryId>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICategoryMapper<CategoryId> _responseMapper;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCategoryCommandHandler(
        ICategoryRepository categoryRepository,
        ICategoryMapper<CategoryId> responseMapper,
        IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _responseMapper = responseMapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<CategoryId> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        if (await _categoryRepository.FindByNameAsync(request.Name) is not null)
        {
            throw new CategoryNameDuplicatedException(request.Name);
        }
        if (request.ParentId is CategoryId parentId && await _categoryRepository.FindByIdAsync(parentId) is null)
        {
            throw new CategoryNotFoundException(parentId);
        }
        var category = new Category(new CategoryId(Guid.NewGuid()), request.Name);
        await _categoryRepository.AddAsync(category, request.ParentId);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return category.Map(_responseMapper);
    }
}
