using Application.Exceptions.Files;
using Application.Images;
using Domain.Files;
using MediatR;

namespace Application.Files.Queries.GetFileById;

internal class GetFileByIdQueryHandler : IRequestHandler<GetFileByIdQuery, FileResponse>
{
    private readonly IFileStorage _fileService;
    private readonly IFileRepository _fileRepository;

    public GetFileByIdQueryHandler(IFileStorage fileService, IFileRepository fileRepository)
    {
        _fileService = fileService;
        _fileRepository = fileRepository;
    }

    public async Task<FileResponse> Handle(GetFileByIdQuery request, CancellationToken cancellationToken)
    {
        StoredFile file = await _fileRepository.FindByIdAsync(request.Id)
            ?? throw new StoredFileNotFoundException(request.Id);
        (byte[] content, string extentsion) = await _fileService.ReadFileAsync(file);
        return new FileResponse(content, extentsion);
    }
}
