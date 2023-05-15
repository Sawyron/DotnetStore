namespace Application.Products.Queries.GetPage;

public record ProductItemResponse(
    Guid Id,
    string Name,
    int Price,
    string Description,
    string ImageUrl,
    Guid CategoryId);
