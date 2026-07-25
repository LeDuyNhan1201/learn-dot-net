namespace BuildingBlocks.Domain.Exceptions.Processors;

public interface IExceptionProcessor
{
    Task ProcessAsync(Exception exception, CancellationToken cancellationToken = default);
}