using MassTransit;

namespace BuildingBlocks.Domain.Contracts;

[ExcludeFromTopology]
public interface IIntegrationEvent
{
    Guid EventId { get; }
    DateTimeOffset OccurredAt { get; }
}