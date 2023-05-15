using Domain.Categories;
using Domain.Files;
using Domain.Products;
using Domain.ProductTypes.Tags.TagOptions;

namespace Application.Products.Commands.DeleteProduct
{
    internal class ProductFileMapper : IProductMapper<StoredFile>
    {
        public StoredFile Map(
            ProductId id,
            string name,
            int price,
            string description,
            IEnumerable<TagOptionId> configuration,
            StoredFile photo,
            CategoryId categoryId) => photo;
    }
}
