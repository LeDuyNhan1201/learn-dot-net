using BuildingBlocks.Infrastructure.Observability.Options;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;

namespace BuildingBlocks.Infrastructure.Observability.Configurations;

public static class LoggingConfiguration
{
    public static void ConfigureLogging(this LoggerProviderBuilder builder, ObservabilityOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(builder);

        switch (options.UseLoggingExporter.ToUpperInvariant())
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