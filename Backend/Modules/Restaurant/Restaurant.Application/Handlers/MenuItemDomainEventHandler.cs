using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Domain.Contracts;
using MassTransit;
using Restaurant.Domain.Contracts;

namespace Restaurant.Application.Handlers;

public static class MenuItemDomainEventHandler
{
    public sealed class Created(IPublishEndpoint publishEndpoint) : IDomainEventHandler<IMenuItemDomainEvent.Created>
    {
        public async Task HandleAsync(IMenuItemDomainEvent.Created domainEvent, 
            CancellationToken cancellationToken)
        {
            var integrationEvent = new IMenuItemIntegrationEvent.Created
            {
                MenuItemName = domainEvent.MenuItemName,
                MenuItemDescription = domainEvent.MenuItemDescription,
                ImageUrl = domainEvent.ImageUrl,
                MenuItemPrice = domainEvent.MenuItemPrice
            };
            
            await publishEndpoint.Publish(integrationEvent, cancellationToken);
        }
    }
}