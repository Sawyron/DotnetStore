using Domain.Files;

namespace Application.Exceptions.Files;

internal class StoredFileNotFoundException : ResourceNotFoundException
{
    public StoredFileNotFoundException(FileId id)
        : base($"Photo with id '{id.Value}' is not found") { }
}
