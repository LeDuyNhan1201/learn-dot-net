using Infrastructure.Options;
using OpenTelemetry.Exporter;
using OpenTelemetry.Trace;

namespace BuildingBlocks.Infrastructure.Observability.Configurations;

public static class TracingConfiguration
{
    public static void ConfigureTracing(this TracerProviderBuilder builder, ObservabilityOptions options)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(options);
        
        builder
            .AddSource(Telemetry.ActivitySourceName)
            .SetSampler(new AlwaysOnSampler())
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation();
            
        switch (options.UseTracingExporter.ToUpperInvariant())
        {
            case "OTLP":
                builder.AddOtlpExporter(otlp =>
                {
                    otlp.Endpoint = new Uri(options.Otlp.Endpoint);
                    otlp.Protocol = OtlpExportProtocol.Grpc;
                });
                break;

            default:
                builder.AddConsoleExporter();
                break;
        }
    }
}