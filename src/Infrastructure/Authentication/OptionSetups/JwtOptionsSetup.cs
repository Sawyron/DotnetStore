using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Infrastructure.Authentication.OptionSetups;

internal class JwtOptionsSetup : IConfigureOptions<JwtOptions>
{
    private const string _jwtSection = "Jwt";
    private readonly IConfiguration _configuration;

    public JwtOptionsSetup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(JwtOptions options)
    {
        _configuration.GetSection(_jwtSection).Bind(options);
    }
}
