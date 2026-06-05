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
    public static IServiceCollection AddAppAuthentication(this IHostApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder.Services);
        ArgumentNullException.ThrowIfNull(builder.Configuration);

        builder.Services.AddScoped<CurrentIdentity>();
        builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddKeycloakWebApi(
                options =>
                {
                    if (builder.Environment.IsEnvironment("Local"))
                    {
                        builder.Configuration.BindKeycloakOptions(options);
                    } 
                    else
                    {
                        options.BindKeycloakOptionsForAot(builder.Configuration);
                    }
                },
                options =>
                {
                    if (builder.Environment.IsEnvironment("Local"))
                    {
                        options.BackchannelHttpHandler = new HttpClientHandler
                        {
                            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                        };
                    }
                    options.ConfigureJwtBearer();
                });

        builder.Services.AddAuthorizationBuilder();

        return builder.Services;
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