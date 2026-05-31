namespace BuildingBlocks.Infrastructure.Observability.Options;

public sealed class ObservabilityOptions
{
    public const string SectionName = "Observability";
    public string UseTracingExporter { get; set; } = "Console";
    public string UseMetricsExporter { get; set; } = "Console";
    public string UseLoggingExporter { get; set; } = "Console";
    public string HistogramAggregation { get; set; } = "Explicit";

    public OtlpOptions? Otlp { get; set; }

    public class OtlpOptions
    {
        public string Endpoint { get; set; } = "http://localhost:4317";
    }
}