namespace WebApi.Tags.Requests;

public sealed record UpdateTagRequest(string Name, Guid CategoryId);
