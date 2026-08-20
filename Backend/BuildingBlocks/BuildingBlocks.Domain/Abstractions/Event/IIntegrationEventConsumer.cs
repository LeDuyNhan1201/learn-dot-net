using MassTransit;

namespace BuildingBlocks.Domain.Abstractions.Event;

public interface IIntegrationEventConsumer<in T> : IConsumer<T>
    where T : class, IIntegrationEvent
{
}