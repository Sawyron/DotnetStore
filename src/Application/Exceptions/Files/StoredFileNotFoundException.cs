using Application.Core;
using Domain.Files;
using Microsoft.AspNetCore.Http;

namespace Application.Exceptions.Files;

internal class StoredFileNotFoundException : ServerApplicationException
{
    public StoredFileNotFoundException(FileId id)
        : base($"Photo with id '{id.Value}' is not found", StatusCodes.Status404NotFound) { }
}
