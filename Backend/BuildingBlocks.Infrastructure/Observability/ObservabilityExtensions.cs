using BuildingBlocks.Infrastructure.Observability.Configurations;
using Infrastructure.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Resources;

namespace BuildingBlocks.Infrastructure.Observability;

public static class ObservabilityExtensions
{
    public static IServiceCollection AddObservability(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(services);
        
        var appOptions = configuration.GetSection(ServerOptions.SectionName).Get<ServerOptions>() 
                      ?? throw new InvalidOperationException("Application configuration is missing.");
        
        var observeOptions = configuration.GetSection(ObservabilityOptions.SectionName).Get<ObservabilityOptions>() 
                      ?? throw new InvalidOperationException("Observability configuration is missing.");
        
        services.AddSingleton<Telemetry>();

        services
            .AddOpenTelemetry()
            .ConfigureResource(resource =>
            {
                resource.AddService(
                    serviceName: appOptions.Name ?? "Unknown",
                    serviceVersion: appOptions.Version,
                    serviceInstanceId: Environment.MachineName);
            })
            .WithTracing(tracing =>
            {
                tracing.ConfigureTracing(observeOptions);
            })
            .WithMetrics(metrics =>
            {
                metrics.ConfigureMetrics(observeOptions);
            })
            .WithLogging(logging =>
            {
                logging.ConfigureLogging(observeOptions);
            });

        return services;
    }

    public static IApplicationBuilder UseMetricsExporter(this IApplicationBuilder app, IConfiguration configuration)
    {
        var options = configuration.GetSection(ObservabilityOptions.SectionName).Get<ObservabilityOptions>()
                      ?? throw new InvalidOperationException("Observability configuration is missing.");
        if ("prometheus".Equals(options.UseMetricsExporter, StringComparison.OrdinalIgnoreCase))
        {
            app.UseOpenTelemetryPrometheusScrapingEndpoint();
        }

        return app;
    }
}