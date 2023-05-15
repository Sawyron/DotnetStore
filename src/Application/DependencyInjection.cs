using Application.Behaviours;
using Application.Categories.Commands.CreateCategory;
using Application.Categories.Queries;
using Application.Categories.Queries.GetPrimaryCategoryPage;
using Application.Files;
using Application.Products;
using Application.Products.Commands.AddTagOption;
using Application.Products.Commands.DeleteProduct;
using Application.Products.Queries.GetById;
using Application.Products.Queries.GetPage;
using Application.TagOptions;
using Application.Tags;
using Application.Tags.Commands.CreateTags;
using Domain.Categories;
using Domain.Files;
using Domain.Products;
using Domain.ProductTypes.Tags.TagOptions;
using Domain.Tags;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            Assembly assembly = typeof(DependencyInjection).Assembly;

            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssembly(assembly);
            });
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            services.AddValidatorsFromAssembly(assembly, includeInternalTypes: true);

            services.AddSingleton<ICategoryMapper<CategoryId>, CategoryIdMapper>();
            services.AddSingleton<ICategoryMapper<CategoryResponse>, CategoryResponseMapper>();
            services.AddSingleton<ICategoryMapper<CategoryItemResponse>, CategoryItemResponseMapper>();

            services.AddSingleton<ITagMapper<TagResponse>, TagResponseMapper>();

            services.AddSingleton<ITagMapper<(TagId, string, CategoryId)>, TagTupleMapper>();

            services.AddSingleton<ITagOptionMapper<(TagOptionId, string, TagId)>, TagOptionTupleMapper>();
            services.AddSingleton<ITagOptionMapper<TagOptionResponse>, TagOptionResponseMapper>();

            services.AddSingleton<IProductMapper<ProductResponse>, ProductResponseMapper>();
            services.AddSingleton<IProductMapper<FileId>, ProductFileIdMapper>();
            services.AddSingleton<IProductMapper<ProductItemResponse>, ProductItemResponseMapper>();
            services.AddSingleton<IProductMapper<StoredFile>, ProductFileMapper>();
            services.AddSingleton<IProductMapper<CategoryId>, ProductCategoryIdMapper>();

            services.AddSingleton<IFileMapper<FileId>, FileIdMapper>();
            return services;
        }
    }
}
