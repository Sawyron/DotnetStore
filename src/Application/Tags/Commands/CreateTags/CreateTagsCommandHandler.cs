using Application.Categories;
using Application.Core;
using Application.Exceptions.Categories;
using Domain.Tags;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Tags.Commands.CreateTags;

public record CreateTagsCommandHandler : IRequestHandler<CreateTagsCommand, IEnumerable<TagResponse>>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ITagRepository _tagRepository;
    private readonly ITagMapper<TagResponse> _responseMapper;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTagsCommandHandler(
        ICategoryRepository categoryRepository,
        ITagRepository tagRepository,
        ITagMapper<TagResponse> responseMapper,
        IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _tagRepository = tagRepository;
        _responseMapper = responseMapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<TagResponse>> Handle(CreateTagsCommand request, CancellationToken cancellationToken)
    {
        if (await _categoryRepository.FindByIdAsync(request.CategoryId) is null)
        {
            throw new CategoryNotFoundException(request.CategoryId, StatusCodes.Status409Conflict);
        }
        List<Tag> tags = request.Tags.
                    Select(d => new Tag(new TagId(Guid.NewGuid()), d.Name, request.CategoryId))
                    .ToList();
        await _tagRepository.AddRangeAsync(tags);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return tags.Select(t => t.Map(_responseMapper)).ToList();
    }
}
