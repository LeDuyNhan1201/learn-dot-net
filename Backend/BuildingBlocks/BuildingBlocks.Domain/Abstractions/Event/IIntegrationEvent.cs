using MassTransit;

namespace BuildingBlocks.Domain.Abstractions.Event;

[ExcludeFromTopology]
public interface IIntegrationEvent
{
    Guid EventId { get; }
    DateTimeOffset OccurredAt { get; }
}