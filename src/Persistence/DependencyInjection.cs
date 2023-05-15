using Application.Categories;
using Application.Core;
using Application.Images;
using Application.Products;
using Application.TagOptions;
using Application.Tags;
using Domain.Categories;
using Domain.Files;
using Domain.Products;
using Domain.ProductTypes.Details;
using Domain.ProductTypes.Tags.TagOptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Data.Categories;
using Persistence.Data.Files;
using Persistence.Data.Images;
using Persistence.Data.Products;
using Persistence.Data.TagOptions;
using Persistence.Data.Tags;

namespace Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationContext>(options =>
            options
                .UseSqlite(configuration.GetConnectionString("Sqlite"))
                .UseSnakeCaseNamingConvention());
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddSingleton<ICategoryMapper<CategoryData>, CategoryDataMapper>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();

        services.AddSingleton<IFileMapper<FileData>, FileDataMapper>();
        services.AddScoped<IFileRepository, FileRepository>();

        services.AddSingleton<IProductMapper<ProductData>, ProductDataMapper>();
        services.AddScoped<IProductRepository, ProductRepository>();

        services.AddSingleton<ITagMapper<TagData>, TagDataMapper>();
        services.AddScoped<ITagRepository, TagRepository>();

        services.AddSingleton<ITagOptionMapper<TagOptionData>, TagOptionDataMapper>();
        services.AddScoped<ITagOptionRepository, TagOptionRepository>();

        return services;
    }
}
