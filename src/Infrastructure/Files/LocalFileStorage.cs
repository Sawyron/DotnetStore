﻿using Application.Files;
using Domain.Files;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Files;

internal class LocalFileStorage : IFileStorage
{
    private readonly string _uploadsFolder;
    private readonly IFileMapper<string> _pathMapper;

    public LocalFileStorage(IConfiguration configuration, IFileMapper<string> pathMapper)
    {
        _uploadsFolder = configuration["ImageFolder"]!;
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
        string path = file.Map(_pathMapper);
        var extension = new string(Path.GetExtension(path).Skip(1).ToArray());
        byte[] content = await File.ReadAllBytesAsync(path);
        return (content, extension);
    }

    async Task<StoredFile> IFileStorage.SaveFileAsync(byte[] content, string extension)
    {
        var id = Guid.NewGuid();
        string path = Path.Combine(_uploadsFolder, id.ToString() + extension);
        using (var stream = new FileStream(path, FileMode.Create))
        {
            await stream.WriteAsync(content);
        }
        return new StoredFile(new FileId(id), path);
    }
}
