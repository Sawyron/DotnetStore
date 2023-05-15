using Application.Core;
using Domain.ProductTypes.Details;

namespace Application.TagOptions.Queries.GetPageByTagId;

public sealed record GetTagOptionsPageByTagIdQuery(
    int Offset,
    int PageSize,
    TagId TagId) : IGetPageQuery<TagOptionResponse>;
