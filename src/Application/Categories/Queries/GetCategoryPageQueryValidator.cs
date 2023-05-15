using Application.Categories.Queries.GetPrimaryCategoryPage;
using Application.Core;
using FluentValidation;

namespace Application.Categories.Queries;

internal class GetCategoryPageQueryValidator : AbstractValidator<GetPageQuery<CategoryItemResponse>>
{
    public GetCategoryPageQueryValidator()
    {
        Include(new GetPageQueryValidator<CategoryItemResponse>());
    }
}
