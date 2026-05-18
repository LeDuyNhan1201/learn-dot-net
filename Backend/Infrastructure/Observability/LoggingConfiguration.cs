using Infrastructure.Options;
using OpenTelemetry.Logs;

namespace Infrastructure.Observability;

public static class LoggingConfiguration
{
    public static void ConfigureLogging(this LoggerProviderBuilder builder, ObservabilityOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(builder);
        
        switch (options.UseLoggingExporter.ToUpperInvariant())
        {
            case "OTLP":
                builder.AddOtlpExporter(exporter =>
                {
                    exporter.Endpoint = new Uri(options.Otlp.Endpoint);
                });
                break;
            
            default:
                builder.AddConsoleExporter();
                break;
        }
    }
}