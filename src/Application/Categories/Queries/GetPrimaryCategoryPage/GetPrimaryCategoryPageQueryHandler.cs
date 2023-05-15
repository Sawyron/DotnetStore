using Application.Core;
using Domain.Categories;
using MediatR;

namespace Application.Categories.Queries.GetPrimaryCategoryPage;

internal class GetPrimaryCategoryPageQueryHandler : IRequestHandler<GetPageQuery<CategoryItemResponse>, IEnumerable<CategoryItemResponse>>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICategoryMapper<CategoryItemResponse> _categoryMapper;

    public GetPrimaryCategoryPageQueryHandler(ICategoryRepository categoryRepository, ICategoryMapper<CategoryItemResponse> categoryMapper)
    {
        _categoryRepository = categoryRepository;
        _categoryMapper = categoryMapper;
    }

    public async Task<IEnumerable<CategoryItemResponse>> Handle(GetPageQuery<CategoryItemResponse> request, CancellationToken cancellationToken)
    {
        var categories = await _categoryRepository.GetAllPrimaryAsync();
        return categories.Select(c => c.Map(_categoryMapper)).ToList();
    }
}
