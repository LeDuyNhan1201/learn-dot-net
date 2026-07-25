namespace BuildingBlocks.Domain.Exceptions.Processors;

public interface IExceptionObserver
{
    Task ObserveAsync(Exception exception, CancellationToken cancellationToken = default);
}