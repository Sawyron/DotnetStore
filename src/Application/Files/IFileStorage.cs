using Domain.Files;

namespace Application.Files;

public interface IFileStorage
{
    Task<StoredFile> SaveFileAsync(byte[] content, string extension);
    Task<(byte[] content, string extension)> ReadFileAsync(StoredFile file);
    bool DeleteFile(StoredFile file);
}
