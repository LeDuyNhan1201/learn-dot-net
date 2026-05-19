using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Infrastructure.Observability;

public sealed class Telemetry : IDisposable
{
    private readonly Meter _meter;
    
    internal const string ActivitySourceName = "LeDuyNhan1201.LearnDotNet";
    internal const string MeterName = "LeDuyNhan1201.LearnDotNet";

    public Telemetry()
    {
        var version = typeof(Telemetry).Assembly.GetName().Version?.ToString();
        _meter = new Meter(MeterName, version);
        ActivitySource = new ActivitySource(ActivitySourceName, version);
    }

    private ActivitySource ActivitySource { get; }
    
    public void Dispose()
    {
        ActivitySource.Dispose();
        _meter.Dispose();
    }
}