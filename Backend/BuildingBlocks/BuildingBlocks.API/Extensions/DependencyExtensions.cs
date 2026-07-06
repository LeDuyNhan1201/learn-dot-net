using BuildingBlocks.Infrastructure.Observability.Options;
using BuildingBlocks.Infrastructure.OpenApi.Options;
using BuildingBlocks.Infrastructure.Persistence.Options;
using BuildingBlocks.Shared.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.API.Extensions;

public static class DependencyExtensions
{
    public static IServiceCollection AddBaseServices(this IServiceCollection services, IConfiguration configuration)
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
            .AddOptions<PostgresOptions>()
            .BindConfiguration(PostgresOptions.Section)
            .ValidateOnStart();

        return services;
    }
}