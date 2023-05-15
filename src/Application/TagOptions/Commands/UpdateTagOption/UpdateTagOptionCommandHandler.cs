using Application.Core;
using Application.Exceptions.TagOptions;
using Application.Exceptions.Tags;
using Application.Tags;
using Domain.ProductTypes.Tags.TagOptions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.TagOptions.Commands.UpdateTagOption;

internal class UpdateTagOptionCommandHandler : IRequestHandler<UpdateTagOptionCommand>
{
    private readonly ITagOptionRepository _tagOptionRepository;
    private readonly ITagRepository _tagRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTagOptionCommandHandler(
        ITagOptionRepository tagOptionRepository,
        ITagRepository tagRepository,
        IUnitOfWork unitOfWork)
    {
        _tagOptionRepository = tagOptionRepository;
        _tagRepository = tagRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateTagOptionCommand request, CancellationToken cancellationToken)
    {
        if (await _tagOptionRepository.FindByIdAsync(request.Id) is null)
        {
            throw new TagOptionNotFoundException(request.Id);
        }
        if (await _tagRepository.FindByIdAsync(request.TagId) is null)
        {
            throw new TagNotFoundException(request.TagId, StatusCodes.Status409Conflict);
        }
        var option = new TagOption(request.Id, request.Value, request.TagId);
        await _tagOptionRepository.UpdateAsync(option);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
