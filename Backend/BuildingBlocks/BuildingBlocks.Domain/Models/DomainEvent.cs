using BuildingBlocks.Domain.Abstractions.Event;

namespace BuildingBlocks.Domain.Models;

public abstract record DomainEvent : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTimeOffset OccurredAt { get; } = DateTimeOffset.UtcNow;
}