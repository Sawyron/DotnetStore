namespace WebApi.Categories.Requests;

public sealed record UpdateCategoryRequest(string Name, Guid ParentId);
