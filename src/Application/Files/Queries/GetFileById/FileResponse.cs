namespace Application.Files.Queries.GetFileById;

public sealed record FileResponse(byte[] Content, string Extension);
