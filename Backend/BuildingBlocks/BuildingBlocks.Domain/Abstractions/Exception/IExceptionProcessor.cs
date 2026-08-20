namespace BuildingBlocks.Domain.Abstractions.Exception;

public interface IExceptionProcessor
{
    Task ProcessAsync(System.Exception exception, CancellationToken cancellationToken = default);
}