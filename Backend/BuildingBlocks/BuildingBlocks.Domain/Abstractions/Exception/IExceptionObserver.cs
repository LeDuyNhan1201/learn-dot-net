namespace BuildingBlocks.Domain.Abstractions.Exception;

public interface IExceptionObserver
{
    Task ObserveAsync(System.Exception exception, CancellationToken cancellationToken = default);
}