using BuildingBlocks.Identity.keycloakAdmin.Options;
using BuildingBlocks.Observability.Options;
using BuildingBlocks.OpenApi.Options;
using BuildingBlocks.Persistence.Options;
using BuildingBlocks.SharedKernel.Options;
using Keycloak.AuthServices.Sdk;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.API.Extensions;

public static class DependencyExtensions
{
    public static IServiceCollection AddBehaviors(this IServiceCollection services, IConfiguration configuration)
    {
        return services;
    }

    public static IServiceCollection AddBaseOptions(this IServiceCollection services)
    {
        services
            .AddOptions<ServerOptions>()
            .BindConfiguration(ServerOptions.Section)
            .ValidateOnStart();

        services
            .AddOptions<ObservabilityOptions>()
            .BindConfiguration(ObservabilityOptions.Section)
            .ValidateOnStart();

        services
            .AddOptions<ApiDocsOptions>()
            .BindConfiguration(ApiDocsOptions.Section)
            .ValidateOnStart();

        services
            .AddOptions<KeycloakAdminClientOptions>()
            .BindConfiguration(KeycloakAdminOptions.Section)
            .ValidateOnStart();

        services
            .AddOptions<PostgresOptions>()
            .BindConfiguration(PostgresOptions.Section)
            .ValidateOnStart();

        return services;
    }
}