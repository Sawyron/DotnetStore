using Application.Categories.Queries.GetPrimaryCategoryPage;
using Application.Core;
using Domain.Categories;
using MediatR;

namespace Application.Categories.Queries.GetCategoryPage;

internal class GetCategoryPageQueryHandler : IRequestHandler<GetPageQuery<CategoryItemResponse>, IEnumerable<CategoryItemResponse>>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICategoryMapper<CategoryItemResponse> _categoryMapper;

    public GetCategoryPageQueryHandler(ICategoryRepository categoryRepository, ICategoryMapper<CategoryItemResponse> categoryMapper)
    {
        _categoryRepository = categoryRepository;
        _categoryMapper = categoryMapper;
    }

    public async Task<IEnumerable<CategoryItemResponse>> Handle(GetPageQuery<CategoryItemResponse> request, CancellationToken cancellationToken)
    {
        var categories = await _categoryRepository.GetPageAsync(request.Offset, request.PageSize);
        return categories.Select(c => c.Map(_categoryMapper)).ToList();
    }
}
