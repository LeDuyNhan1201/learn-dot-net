using BuildingBlocks.Domain.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Messaging.Events;

public sealed class DomainEventExecutor(IServiceProvider serviceProvider) : IDomainEventExecutor
{
    public async Task ExecuteAsync(IEnumerable<IDomainEvent> events,
        CancellationToken cancellationToken)
    {
        foreach (var domainEvent in events)
        {
            var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());

            var handlers = serviceProvider.GetServices(handlerType);

            foreach (var handler in handlers)
            {
                await ((dynamic)handler!).HandleAsync((dynamic)domainEvent, cancellationToken);
            }
        }
    }
}