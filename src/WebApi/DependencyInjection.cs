using WebApi.Errors;
using WebApi.Errors.ErrorHandlers;

namespace WebApi
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddExceptionHandling(this IServiceCollection services)
        {
            services.AddSingleton<ResourceNotFoundExceptionHandler>();
            services.AddSingleton<ResourceDublicationExceptionHandler>();
            services.AddSingleton<ResourceStateConflictExceptionHandler>();
            services.AddSingleton<ValidationExceptionHandler>();
            services.AddSingleton<IErrorHandler>(s =>
            {
                var provider = new AggregateErrorHandler(new IErrorHandler[]
                {
                    s.GetService<ResourceNotFoundExceptionHandler>()!,
                    s.GetService<ResourceDublicationExceptionHandler>()!,
                    s.GetService<ResourceStateConflictExceptionHandler>()!,
                    s.GetService<ValidationExceptionHandler>()!
                });
                return provider;
            });
            return services;
        }
    }
}
