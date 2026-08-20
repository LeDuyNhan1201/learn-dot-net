using System.Diagnostics;
using BuildingBlocks.Domain.Abstractions.Exception;
using BuildingBlocks.Observability.Meters;

namespace BuildingBlocks.Observability.Exceptions;

public sealed class TelemetryExceptionObserver(Telemetry telemetry) : IExceptionObserver
{
    public Task ObserveAsync(Exception exception, CancellationToken cancellationToken = default)
    {
        var activity = Activity.Current;

        activity?.AddException(exception);

        activity?.SetStatus(
            ActivityStatusCode.Error,
            exception.Message);

        telemetry.ExceptionCounter.Add(
            1,
            KeyValuePair.Create<string, object?>(
                "exception.type",
                exception.GetType().Name));

        return Task.CompletedTask;
    }
}