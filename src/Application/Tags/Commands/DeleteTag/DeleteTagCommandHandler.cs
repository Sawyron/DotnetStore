using Application.Core;
using Application.Exceptions.Tags;
using MediatR;

namespace Application.Tags.Commands.DeleteTag;

internal class DeleteTagCommandHandler : IRequestHandler<DeleteTagCommand>
{
    private readonly ITagRepository _tagRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteTagCommandHandler(ITagRepository tagRepository, IUnitOfWork unitOfWork)
    {
        _tagRepository = tagRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteTagCommand request, CancellationToken cancellationToken)
    {
        bool result = await _tagRepository.DeleteAsync(request.Id);
        if (!result)
        {
            throw new TagNotFoundException(request.Id);
        }
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
