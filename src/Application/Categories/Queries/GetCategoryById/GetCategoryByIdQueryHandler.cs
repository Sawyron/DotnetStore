using Application.Categories.Queries.GetPrimaryCategoryPage;
using Application.Exceptions.Categories;
using Domain.Categories;
using MediatR;

namespace Application.Categories.Queries.GetCategoryById;

internal class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, CategoryResponse>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICategoryMapper<CategoryResponse> _categoryMapper;
    private readonly ICategoryMapper<CategoryItemResponse> _childMapper;

    public GetCategoryByIdQueryHandler(
        ICategoryRepository categoryRepository,
        ICategoryMapper<CategoryResponse> mapper,
        ICategoryMapper<CategoryItemResponse> childMapper)
    {
        _categoryRepository = categoryRepository;
        _categoryMapper = mapper;
        _childMapper = childMapper;
    }

    public async Task<CategoryResponse> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        Category category = await _categoryRepository.FindByIdAsync(request.Id)
            ?? throw new CategoryNotFoundException(request.Id);
        var children = await _categoryRepository.FindChildrenAsync(request.Id);
        CategoryResponse categoryResponse = category.Map(_categoryMapper);
        return categoryResponse with { Children = children.Select(c => c.Map(_childMapper)) };
    }
}
