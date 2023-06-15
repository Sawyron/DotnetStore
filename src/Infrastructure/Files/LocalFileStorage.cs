using Application.Files;
using Domain.Files;
using Microsoft.AspNetCore.Hosting;

namespace Infrastructure.Files;

internal class LocalFileStorage : IFileStorage
{
    private readonly string _uploadsFolder;
    private readonly IFileMapper<string> _pathMapper;

    public LocalFileStorage(IFileMapper<string> pathMapper, IWebHostEnvironment enviroment)
    {
        _uploadsFolder = Path.Combine(enviroment.WebRootPath, "Uploads");
        _pathMapper = pathMapper;
    }

    public bool DeleteFile(StoredFile file)
    {
        string path = Path.Combine(_uploadsFolder, file.Map(_pathMapper));
        if (!File.Exists(path))
        {
            return false;
        }
        File.Delete(path);
        return true;
    }

    public async Task<(byte[] content, string extension)> ReadFileAsync(StoredFile file)
    {
        string path = Path.Combine(_uploadsFolder, file.Map(_pathMapper));
        var extension = new string(Path.GetExtension(path).Skip(1).ToArray());
        byte[] content = await File.ReadAllBytesAsync(path);
        return (content, extension);
    }

    public async Task<StoredFile> SaveFileAsync(byte[] content, string extension)
    {
        var id = Guid.NewGuid();
        string path = id.ToString() + extension;
        using (var stream = new FileStream(Path.Combine(_uploadsFolder, path), FileMode.Create))
        {
            await stream.WriteAsync(content);
        }
        return new StoredFile(new FileId(id), path);
    }
}
