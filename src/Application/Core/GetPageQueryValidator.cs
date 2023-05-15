using FluentValidation;

namespace Application.Core;

internal class GetPageQueryValidator<T> : AbstractValidator<IGetPageQuery<T>>
{
    public GetPageQueryValidator()
    {
        RuleFor(q => q.Offset)
            .GreaterThanOrEqualTo(0);
        RuleFor(q => q.PageSize)
            .GreaterThanOrEqualTo(0);
    }
}
