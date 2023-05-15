using Domain.Files;

namespace Persistence.Data.Images;

internal class FileDataMapper : IFileMapper<FileData>
{
    public FileData Map(FileId id, string path) => new()
    {
        Id = id,
        Path = path
    };
}
