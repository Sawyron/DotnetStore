using Application.Core;
using FluentValidation;

namespace Application.Products.Queries.GetPage;

internal class GetProductPageByCategoryIdQueryValidator : AbstractValidator<GetProductPageByCategoryIdQuery>
{
    public GetProductPageByCategoryIdQueryValidator()
    {
        Include(new GetPageQueryValidator<ProductItemResponse>());
    }
}
