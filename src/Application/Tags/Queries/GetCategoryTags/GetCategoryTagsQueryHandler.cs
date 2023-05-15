using Application.Tags.Commands.CreateTags;
using Domain.ProductTypes.Details;
using MediatR;

namespace Application.Tags.Queries.GetCategoryTags;

internal class GetCategoryTagsQueryHandler : IRequestHandler<GetCategoryTagsQuery, IEnumerable<TagResponse>>
{
    private readonly ITagRepository _tagRepository;
    private readonly ITagMapper<TagResponse> _tagMapper;

    public GetCategoryTagsQueryHandler(ITagRepository tagRepository, ITagMapper<TagResponse> tagMapper)
    {
        _tagRepository = tagRepository;
        _tagMapper = tagMapper;
    }

    public async Task<IEnumerable<TagResponse>> Handle(GetCategoryTagsQuery request, CancellationToken cancellationToken)
    {
        var tags = await _tagRepository.FindCategoryTags(request.CategoryId);
        return tags.Select(t => t.Map(_tagMapper)).ToList();
    }
}
