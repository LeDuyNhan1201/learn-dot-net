using BuildingBlocks.Domain.Enumerations;
using BuildingBlocks.Identity.Configurations;
using BuildingBlocks.Identity.Models;
using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Common;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BuildingBlocks.Identity.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddAuthenticationWithAuthorization<T>(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddScoped<CurrentIdentity>();
        services.AddTransient<IClaimsTransformation, KeycloakRoleClaimsTransformation>();

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddKeycloakWebApi(options => configuration.BindKeycloakOptions(options),
                options =>
                {
                    if (environment.IsEnvironment("Local"))
                        options.BackchannelHttpHandler = new HttpClientHandler
                        {
                            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                        };

                    options.ConfigureJwtBearer<T>();
                });

        services.AddAuthorizationBuilder()
            .AddPolicy(nameof(KeycloakUserGroup.Administrators),
                policy => policy.RequireRole(nameof(KeycloakUserRole.Admin).ToLower()))
            .AddPolicy(nameof(KeycloakUserGroup.Customers),
                policy => policy.RequireRole(nameof(KeycloakUserRole.Customer).ToLower()));

        return services;
    }

    private static void BindKeycloakOptionsForAot(this KeycloakAuthenticationOptions options,
        IConfiguration configuration)
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