using Domain.Files;

namespace Infrastructure.Files;

internal class ImagePathMapper : IFileMapper<string>
{
    public string Map(FileId id, string path) => path;
}
