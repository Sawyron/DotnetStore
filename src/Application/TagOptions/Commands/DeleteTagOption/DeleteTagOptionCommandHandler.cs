using Application.Core;
using Application.Exceptions.TagOptions;
using MediatR;

namespace Application.TagOptions.Commands.DeleteTagOption;

internal class DeleteTagOptionCommandHandler : IRequestHandler<DeleteTagOptionCommand>
{
    private readonly ITagOptionRepository _tagOptionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteTagOptionCommandHandler(ITagOptionRepository tagOptionRepository, IUnitOfWork unitOfWork)
    {
        _tagOptionRepository = tagOptionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteTagOptionCommand request, CancellationToken cancellationToken)
    {
        if (!await _tagOptionRepository.DeleteAsync(request.Id))
        {
            throw new TagOptionNotFoundException(request.Id);
        }
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
