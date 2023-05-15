using Application.Core;
using Application.Exceptions.Tags;
using Application.Tags;
using Domain.ProductTypes.Tags.TagOptions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.TagOptions.Commands.CreateTagOption;

internal class CreateTagOptonCommandHandler : IRequestHandler<CreateTagOptionCommand, TagOptionId>
{
    private readonly ITagOptionRepository _tagOptionRepository;
    private readonly ITagRepository _tagRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTagOptonCommandHandler(ITagOptionRepository tagOptionRepository, ITagRepository tagRepository, IUnitOfWork unitOfWork)
    {
        _tagOptionRepository = tagOptionRepository;
        _tagRepository = tagRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<TagOptionId> Handle(CreateTagOptionCommand request, CancellationToken cancellationToken)
    {
        if (await _tagRepository.FindByIdAsync(request.TagId) is null)
        {
            throw new TagNotFoundException(request.TagId, StatusCodes.Status404NotFound);
        }
        var tagOptionId = new TagOptionId(Guid.NewGuid());
        var tagOption = new TagOption(tagOptionId, request.Value, request.TagId);
        await _tagOptionRepository.AddAsync(tagOption);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return tagOptionId;
    }
}
