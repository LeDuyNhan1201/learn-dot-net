using Infrastructure.Options;
using OpenTelemetry.Trace;

namespace Infrastructure.Observability;

public static class TracingConfiguration
{
    public static void ConfigureTracing(this TracerProviderBuilder builder, ObservabilityOptions options)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(options);
        
        builder
            .AddSource(InstrumentationSource.ActivitySourceName)
            .SetSampler(new AlwaysOnSampler())
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation();
            
        switch (options.UseTracingExporter.ToUpperInvariant())
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