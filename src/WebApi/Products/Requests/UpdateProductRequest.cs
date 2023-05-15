namespace WebApi.Products.Requests;

public sealed record UpdateProductRequest(
    string Name,
    int Price,
    string Description);
