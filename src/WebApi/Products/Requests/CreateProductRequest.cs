namespace WebApi.Products.Requests;

public record CreateProductRequest(
    string Name,
    int Price,
    string Description,
    IEnumerable<Guid> Configuration,
    IFormFile Photo,
    Guid CategoryId);
