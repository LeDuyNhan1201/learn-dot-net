using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Infrastructure.Observability;

public sealed class InstrumentationSource : IDisposable
{
    private readonly Meter _meter;
    
    internal const string ActivitySourceName = "Examples.AspNetCore";
    internal const string MeterName = "Examples.AspNetCore";

    public InstrumentationSource()
    {
        var version = typeof(InstrumentationSource).Assembly.GetName().Version?.ToString();
        _meter = new Meter(MeterName, version);
        ActivitySource = new ActivitySource(ActivitySourceName, version);
        FreezingDaysCounter = _meter.CreateCounter<long>("weather.days.freezing", description: "The number of days where the temperature is below freezing");
    }

    private ActivitySource ActivitySource { get; }

    public Counter<long> FreezingDaysCounter { get; }

    public void Dispose()
    {
        ActivitySource.Dispose();
        _meter.Dispose();
    }
}