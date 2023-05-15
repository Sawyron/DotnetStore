using Application.Categories.Queries.GetPrimaryCategoryPage;

namespace Application.Categories.Queries;

public sealed record CategoryResponse(Guid Id, string Name, IEnumerable<CategoryItemResponse> Children);
