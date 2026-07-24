namespace BuildingBlocks.Domain.Contracts;

public interface IDomainEvent
{
    Guid EventId { get; }
    DateTimeOffset OccurredAt { get; }
}