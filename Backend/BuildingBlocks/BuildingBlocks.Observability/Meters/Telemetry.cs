using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace BuildingBlocks.Observability.Meters;

public sealed class Telemetry : IDisposable
{
    public const string ActivitySourceName = "LeDuyNhan1201.LearnDotNet";
    public const string MeterName = "LeDuyNhan1201.LearnDotNet";

    public Telemetry()
    {
        var version = typeof(Telemetry).Assembly.GetName().Version?.ToString();

        ActivitySource = new ActivitySource(ActivitySourceName, version);

        Meter = new Meter(MeterName, version);

        ExceptionCounter =
            Meter.CreateCounter<long>("exceptions");
    }

    public ActivitySource ActivitySource { get; }

    public Meter Meter { get; }

    public Counter<long> ExceptionCounter { get; }

    public void Dispose()
    {
        ActivitySource.Dispose();
        Meter.Dispose();
    }

    public Activity? StartActivity(
        string name,
        ActivityKind kind = ActivityKind.Internal)
    {
        return ActivitySource.StartActivity(name, kind);
    }
}