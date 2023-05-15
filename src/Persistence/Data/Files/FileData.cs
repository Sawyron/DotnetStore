using Domain.Files;

namespace Persistence.Data.Images;

internal class FileData
{
    public FileId Id { get; set; } = new FileId(Guid.NewGuid());
    public string Path { get; set; } = string.Empty;

    public StoredFile ToDomainModel() => new(Id, Path);
}
