namespace BuildingBlocks.Domain.Contracts;

public interface IDomainEventExecutor
{
    Task ExecuteAsync(IEnumerable<IDomainEvent> events,
        CancellationToken cancellationToken);
}