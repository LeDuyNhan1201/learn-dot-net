namespace BuildingBlocks.Domain.Abstractions.Event;

public interface IDomainEventExecutor
{
    Task ExecuteAsync(IEnumerable<IDomainEvent> events, CancellationToken cancellationToken);
}