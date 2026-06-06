using Microsoft.Extensions.Configuration;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;

namespace BuildingBlocks.Infrastructure.Observability.Configurations;

public static class LoggingConfiguration
{
    public static void ConfigureLogging(this LoggerProviderBuilder builder, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(builder);

        var useLoggingExporter = configuration["Observability:UseLoggingExporter"] ?? "console";
        var grpcOtlpEndpoint = configuration["Observability:Otlp:Endpoint"] ?? "http://localhost:4317";

        switch (useLoggingExporter.ToUpperInvariant())
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