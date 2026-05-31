using BuildingBlocks.Infrastructure.Authentication.Extensions;
using BuildingBlocks.Infrastructure.Observability.Options;
using BuildingBlocks.Infrastructure.OpenApi.Extensions;
using BuildingBlocks.Infrastructure.OpenApi.Options;
using BuildingBlocks.Shared.Options;
using Keycloak.AuthServices.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.API.Extensions;

public static class DependencyExtensions
{
    public static IServiceCollection AddBaseServices(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddBaseOptions()
            .AddBaseInfrastructure(configuration);

        return services;
    }

    private static IServiceCollection AddBaseOptions(this IServiceCollection services)
    {
        services
            .AddOptions<ServerOptions>()
            .BindConfiguration(ServerOptions.SectionName)
            .ValidateOnStart();

        services
            .AddOptions<ObservabilityOptions>()
            .BindConfiguration(ObservabilityOptions.SectionName)
            .ValidateOnStart();

        services
            .AddOptions<ApiDocsOptions>()
            .BindConfiguration(ApiDocsOptions.SectionName)
            .ValidateOnStart();

        services
            .AddOptions<KeycloakAuthenticationOptions>()
            .BindConfiguration(KeycloakAuthenticationOptions.Section)
            .ValidateOnStart();

        return services;
    }

    private static IServiceCollection AddBaseInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services
            // .AddObservability(configuration)
            .AddScalarOpenApi()
            .AddAppAuthentication(configuration);

        return services;
    }
}