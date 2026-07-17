using System.Diagnostics.Metrics;
using BuildingBlocks.Observability.Meters;
using Microsoft.Extensions.Configuration;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;

namespace BuildingBlocks.Observability.Configurations;

public static class MetricsConfiguration
{
    public static void ConfigureMetrics(this MeterProviderBuilder builder, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder
            .AddMeter(Telemetry.MeterName)
            .AddRuntimeInstrumentation()
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation();

        var histogramAggregation = configuration["Observability:HistogramAggregation"] ?? "Explicit";
        var useMetricsExporter = configuration["Observability:UseMetricsExporter"] ?? "console";
        var grpcOtlpEndpoint = configuration["Observability:Otlp:Endpoint"] ?? "http://localhost:4317";

        switch (histogramAggregation.ToUpperInvariant())
        {
            case "EXPONENTIAL":
                builder.AddView(instrument => instrument.GetType().GetGenericTypeDefinition() == typeof(Histogram<>)
                    ? new Base2ExponentialBucketHistogramConfiguration()
                    : null);
                break;
        }

        switch (useMetricsExporter.ToUpperInvariant())
        {
            case "OTLP":
                builder.AddOtlpExporter(otlp =>
                {
                    otlp.Endpoint = new Uri(grpcOtlpEndpoint);
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