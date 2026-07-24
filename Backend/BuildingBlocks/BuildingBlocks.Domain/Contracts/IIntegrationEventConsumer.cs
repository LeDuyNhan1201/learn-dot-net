using MassTransit;

namespace BuildingBlocks.Domain.Contracts;

public interface IIntegrationEventConsumer<in T> : IConsumer<T>
where T : class, IIntegrationEvent
{
}