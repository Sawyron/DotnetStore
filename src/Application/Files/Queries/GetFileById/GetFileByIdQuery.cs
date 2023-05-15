using Domain.Files;
using MediatR;

namespace Application.Files.Queries.GetFileById
{
    public sealed record GetFileByIdQuery(FileId Id) : IRequest<FileResponse>;
}
