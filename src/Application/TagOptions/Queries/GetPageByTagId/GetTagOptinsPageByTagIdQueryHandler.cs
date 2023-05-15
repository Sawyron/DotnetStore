using Domain.ProductTypes.Tags.TagOptions;
using MediatR;

namespace Application.TagOptions.Queries.GetPageByTagId;

internal class GetTagOptinsPageByTagIdQueryHandler : IRequestHandler<GetTagOptionsPageByTagIdQuery, IEnumerable<TagOptionResponse>>
{
    private readonly ITagOptionRepository _tagOptionRepository;
    private readonly ITagOptionMapper<TagOptionResponse> _tagOptionMapper;

    public GetTagOptinsPageByTagIdQueryHandler(ITagOptionRepository tagOptionRepository, ITagOptionMapper<TagOptionResponse> tagOptionMapper)
    {
        _tagOptionRepository = tagOptionRepository;
        _tagOptionMapper = tagOptionMapper;
    }

    public async Task<IEnumerable<TagOptionResponse>> Handle(GetTagOptionsPageByTagIdQuery request, CancellationToken cancellationToken)
    {
        var options = await _tagOptionRepository.GetTagOptionsPage(
            request.TagId,
            request.Offset,
            request.PageSize);
        return options.Select(o => o.Map(_tagOptionMapper));
    }
}
