using Domain.Files;
using MediatR;

namespace Application.Files.Commands.CreateFile;

public sealed record CreateFileCommand(byte[] Content, string Extension) : IRequest<FileId>;
