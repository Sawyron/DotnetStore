using Application.Categories;
using Application.Core;
using Application.Exceptions.Categories;
using Application.Exceptions.Tags;
using Domain.Tags;
using MediatR;

namespace Application.Tags.Commands.CreateTag;

internal class CreateTagCommandHandler : IRequestHandler<CreateTagCommand, TagId>
{
    private readonly ITagRepository _tagRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTagCommandHandler(
        ITagRepository tagRepository,
        ICategoryRepository categoryRepository,
        IUnitOfWork unitOfWork)
    {
        _tagRepository = tagRepository;
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<TagId> Handle(CreateTagCommand request, CancellationToken cancellationToken)
    {
        if (await _categoryRepository.FindByIdAsync(request.CategoryId) is null)
        {
            throw new CategoryNotFoundException(request.CategoryId);
        }
        if (await _tagRepository.FindByNameInCategory(request.Name, request.CategoryId) is not null)
        {
            throw new TagNameConflictException(request.Name, request.CategoryId);
        }
        var tagId = new TagId(Guid.NewGuid());
        var tag = new Tag(tagId, request.Name, request.CategoryId);
        await _tagRepository.AddAsync(tag);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return tagId;
    }
}
