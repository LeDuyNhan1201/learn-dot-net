using BuildingBlocks.Domain.Exceptions.Processors;

namespace BuildingBlocks.Application.Exceptions;

public sealed class ExceptionProcessor(IEnumerable<IExceptionObserver> observers) : IExceptionProcessor
{
    public async Task ProcessAsync(Exception exception, CancellationToken cancellationToken = default)
    {
        foreach (var observer in observers) await observer.ObserveAsync(exception, cancellationToken);
    }
}