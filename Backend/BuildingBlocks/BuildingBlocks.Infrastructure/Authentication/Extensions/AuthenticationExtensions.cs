using BuildingBlocks.Infrastructure.Authentication.Models;
using Keycloak.AuthServices.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Infrastructure.Authentication.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddAppAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddScoped<CurrentIdentity>()
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddKeycloakWebApi(configuration.GetSection(KeycloakAuthenticationOptions.Section), options =>
            {
                configuration.GetSection(KeycloakAuthenticationOptions.Section).Bind(options);
                options.TokenValidationParameters.ValidIssuer = options.Authority?.TrimEnd('/');
                options.ConfigureJwtBearer();
            });

        services.AddAuthorization();

        return services;
    }
}