namespace WebApi.Tags.Requests;

public sealed record CreateTagRequest(string Name, Guid CategoryId);
