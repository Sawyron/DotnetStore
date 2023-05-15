namespace Application.Products;

public record ProductResponse(
    Guid Id,
    string Name,
    int Price,
    string Description,
    string ImageUrl,
    IEnumerable<ProductTagOptionResponse> Configuration,
    Guid CategoryId);
