using BuildingBlocks.Infrastructure.Authentication.Models;
using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BuildingBlocks.Infrastructure.Authentication.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddAuthenticationWithAuthorization(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddScoped<CurrentIdentity>();
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddKeycloakWebApi(
                options =>
                {
                    if (environment.IsEnvironment("Local"))
                        configuration.BindKeycloakOptions(options);
                    else
                        options.BindKeycloakOptionsForAot(configuration);
                },
                options =>
                {
                    if (environment.IsEnvironment("Local"))
                        options.BackchannelHttpHandler = new HttpClientHandler
                        {
                            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                        };
                    options.ConfigureJwtBearer();
                });

        services.AddAuthorizationBuilder();

        return services;
    }

    private static void BindKeycloakOptionsForAot(this KeycloakAuthenticationOptions options, IConfiguration configuration)
    {
        options.Realm = configuration["Keycloak:Realm"]!;
        options.AuthServerUrl = configuration["Keycloak:AuthServerUrl"]!;
        options.Resource = configuration["Keycloak:Resource"]!;
        options.Credentials.Secret = configuration["Keycloak:Credentials:Secret"]!;
        options.SslRequired = configuration["Keycloak:SslRequired"]!;
        options.VerifyTokenAudience = bool.Parse(configuration["Keycloak:VerifyTokenAudience"]!);
        options.Audience = configuration["Keycloak:Audience"]!;
    }
}