using System.Diagnostics.Metrics;
using BuildingBlocks.Application.Options;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;

namespace BuildingBlocks.Infrastructure.Observability.Configurations;

public static class MetricsConfiguration
{
    public static void ConfigureMetrics(this MeterProviderBuilder builder, ObservabilityOptions options)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(options);
        
        builder
            .AddMeter(Telemetry.MeterName)
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
                builder.AddOtlpExporter(otlp =>
                {
                    otlp.Endpoint = new Uri(options.Otlp!.Endpoint);
                    otlp.Protocol = OtlpExportProtocol.Grpc;
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