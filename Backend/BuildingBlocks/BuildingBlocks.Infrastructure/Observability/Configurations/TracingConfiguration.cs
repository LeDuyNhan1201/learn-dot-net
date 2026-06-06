using BuildingBlocks.Infrastructure.Observability.Meters;
using Microsoft.Extensions.Configuration;
using OpenTelemetry.Exporter;
using OpenTelemetry.Trace;

namespace BuildingBlocks.Infrastructure.Observability.Configurations;

public static class TracingConfiguration
{
    public static void ConfigureTracing(this TracerProviderBuilder builder, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder
            .AddSource(Telemetry.ActivitySourceName)
            .SetSampler(new AlwaysOnSampler())
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation();

        var useTracingExporter = configuration["Observability:UseTracingExporter"] ?? "console";
        var grpcOtlpEndpoint = configuration["Observability:Otlp:Endpoint"] ?? "http://localhost:4317";

        switch (useTracingExporter.ToUpperInvariant())
        {
            case "OTLP":
                builder.AddOtlpExporter(otlp =>
                {
                    otlp.Endpoint = new Uri(grpcOtlpEndpoint);
                    otlp.Protocol = OtlpExportProtocol.Grpc;
                });
                break;

            default:
                builder.AddConsoleExporter();
                break;
        }
    }
}