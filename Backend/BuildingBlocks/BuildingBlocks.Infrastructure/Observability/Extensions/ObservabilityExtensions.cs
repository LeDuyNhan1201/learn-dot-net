using BuildingBlocks.Infrastructure.Observability.Configurations;
using BuildingBlocks.Infrastructure.Observability.Meters;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Resources;

namespace BuildingBlocks.Infrastructure.Observability.Extensions;

public static class ObservabilityExtensions
{
    public static IServiceCollection AddObservability(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<Telemetry>();

        services
            .AddOpenTelemetry()
            .ConfigureResource(resource =>
            {
                resource.AddService(
                    configuration["Server:Name"] ?? "Unknown",
                    serviceVersion: configuration["Server:Version"] ?? "0.0.0",
                    serviceInstanceId: Environment.MachineName);
            })
            .WithTracing(tracing => { tracing.ConfigureTracing(configuration); })
            .WithMetrics(metrics => { metrics.ConfigureMetrics(configuration); })
            .WithLogging(logging => { logging.ConfigureLogging(configuration); });

        return services;
    }

    public static IApplicationBuilder UseMetricsExporter(this IApplicationBuilder app, IConfiguration configuration)
    {
        if ("prometheus".Equals(configuration["Observability:UseMetricsExporter"] ?? "console", StringComparison.OrdinalIgnoreCase))
            app.UseOpenTelemetryPrometheusScrapingEndpoint();

        return app;
    }
}