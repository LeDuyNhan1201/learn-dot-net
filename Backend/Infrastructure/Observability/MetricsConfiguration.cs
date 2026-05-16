using System.Diagnostics.Metrics;
using Infrastructure.Options;
using OpenTelemetry.Metrics;

namespace Infrastructure.Observability;

public static class MetricsConfiguration
{
    public static void ConfigureMetrics(this MeterProviderBuilder builder, ObservabilityOptions options)
    {
        builder
            .AddMeter(InstrumentationSource.MeterName)
            .AddRuntimeInstrumentation()
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation();

        switch (options.HistogramAggregation.ToUpperInvariant())
        {
            case "EXPONENTIAL":
                builder.AddView(instrument => instrument.GetType().GetGenericTypeDefinition() == typeof(Histogram<>) 
                    ? new Base2ExponentialBucketHistogramConfiguration() : null);
                break;
        }
        
        switch (options.UseMetricsExporter.ToUpperInvariant())
        {
            case "OTLP":
                builder.AddOtlpExporter(exporter =>
                {
                    exporter.Endpoint = new Uri(options.Otlp.Endpoint);
                });
                break;
            
            case "PROMETHEUS":
                builder.AddPrometheusExporter();
                break;

            default:
                builder.AddConsoleExporter();
                break;
        }
    }
}