using Application.Categories;
using Application.Core;
using Application.Exceptions.Categories;
using Application.Exceptions.Tags;
using Domain.ProductTypes.Details;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Tags.Commands.UpdateTag;

internal class UpdateTagCommandHandler : IRequestHandler<UpdateTagCommand>
{
    private readonly ITagRepository _tagRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTagCommandHandler(
        ITagRepository tagRepository,
        ICategoryRepository categoryRepository,
        IUnitOfWork unitOfWork)
    {
        _tagRepository = tagRepository;
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateTagCommand request, CancellationToken cancellationToken)
    {
        if (await _tagRepository.FindByIdAsync(request.Id) is null)
        {
            throw new TagNotFoundException(request.Id);
        }
        if (await _categoryRepository.FindByIdAsync(request.CategoryId) is null)
        {
            throw new CategoryNotFoundException(request.CategoryId, StatusCodes.Status409Conflict);
        }
        var tag = new Tag(request.Id, request.Name, request.CategoryId);
        await _tagRepository.UpdateAsync(tag);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
