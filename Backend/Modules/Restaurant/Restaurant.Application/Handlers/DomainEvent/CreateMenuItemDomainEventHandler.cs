using BuildingBlocks.Domain.Abstractions.Event;
using MassTransit;
using Restaurant.Domain.Contracts.DomainEvents;
using Restaurant.Domain.Contracts.IntegrationEvents;

namespace Restaurant.Application.Handlers.DomainEvent;

public sealed class CreateMenuItemDomainEventHandler(IPublishEndpoint publishEndpoint)
    : IDomainEventHandler<MenuItemCreatedDomainEvent>
{
    public async Task HandleAsync(MenuItemCreatedDomainEvent createdDomainEvent, CancellationToken cancellationToken)
    {
        var integrationEvent = new MenuItemCreatedIntegrationEvent
        {
            MenuItemName = createdDomainEvent.MenuItemName,
            MenuItemDescription = createdDomainEvent.MenuItemDescription,
            ImageUrl = createdDomainEvent.ImageUrl,
            MenuItemPrice = createdDomainEvent.MenuItemPrice
        };

        await publishEndpoint.Publish(integrationEvent, cancellationToken);
    }
}