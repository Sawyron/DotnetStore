using Domain.Files;

namespace Application.Files;

internal class FileIdMapper : IFileMapper<FileId>
{
    public FileId Map(FileId id, string path) => id;
}
