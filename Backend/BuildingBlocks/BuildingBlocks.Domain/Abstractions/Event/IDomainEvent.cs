namespace BuildingBlocks.Domain.Abstractions.Event;

public interface IDomainEvent
{
    Guid EventId { get; }
    DateTimeOffset OccurredAt { get; }
}