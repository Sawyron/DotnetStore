using Application.Authentication;
using Application.Files;
using Domain.Files;
using Infrastructure.Authentication;
using Infrastructure.Authentication.OptionSetups;
using Infrastructure.Files;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IFileMapper<string>, ImagePathMapper>();
        services.AddSingleton<IFileStorage, LocalFileStorage>();
        return services;
    }

    public static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();
        services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
        services.ConfigureOptions<JwtBearerOptionsSetup>();
        services.AddSingleton<IJwtProvider, JwtProvider>();
        return services;
    }
}
