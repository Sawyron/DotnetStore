using Domain.Categories;
using Domain.Files;
using Domain.ProductTypes.Tags.TagOptions;

namespace Domain.Products;

public sealed class Product
{
    private readonly ProductId _id;
    private readonly string _name;
    private readonly int _price;
    private readonly string _description;
    private readonly StoredFile _photo;
    private readonly List<TagOptionId> _configuration;
    private readonly CategoryId _categoryId;

    public Product(
        ProductId id,
        string name,
        int price,
        string description,
        StoredFile photo,
        IEnumerable<TagOptionId> configuration,
        CategoryId categoryId)
    {
        _id = id;
        _name = name;
        _price = price;
        _description = description;
        _photo = photo;
        _configuration = new List<TagOptionId>(configuration);
        _categoryId = categoryId;
    }

    public T Map<T>(IProductMapper<T> mapper) =>
        mapper.Map(
            _id,
            _name,
            _price,
            _description,
            _configuration,
            _photo,
            _categoryId);

    public Product Update(
        string? name = null,
        int? price = null,
        string? description = null,
        StoredFile? photo = null,
        IEnumerable<TagOptionId>? configuration = null,
        CategoryId? categoryId = null) => new(
            _id,
            name ?? _name,
            price ?? _price,
            description ?? _description,
            photo ?? _photo,
            configuration ?? _configuration,
            categoryId ?? _categoryId);
}
