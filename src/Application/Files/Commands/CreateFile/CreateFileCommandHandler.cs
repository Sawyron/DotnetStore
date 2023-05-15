using Application.Core;
using Application.Images;
using Domain.Files;
using MediatR;

namespace Application.Files.Commands.CreateFile;

internal class CreateFileCommandHandler : IRequestHandler<CreateFileCommand, FileId>
{
    private readonly IFileStorage _fileStorage;
    private readonly IFileRepository _fileRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileMapper<FileId> _mapper;

    public CreateFileCommandHandler(
        IFileStorage fileStorage,
        IFileRepository fileRepository,
        IUnitOfWork unitOfWork,
        IFileMapper<FileId> mapper)
    {
        _fileStorage = fileStorage;
        _fileRepository = fileRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<FileId> Handle(CreateFileCommand request, CancellationToken cancellationToken)
    {
        StoredFile file = await _fileStorage.SaveFileAsync(request.Content, request.Extension);
        await _fileRepository.AddAsync(file);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return file.Map(_mapper);
    }
}
