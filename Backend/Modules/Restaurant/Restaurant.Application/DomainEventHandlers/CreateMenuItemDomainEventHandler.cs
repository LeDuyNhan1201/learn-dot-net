using BuildingBlocks.Domain.Contracts;
using MassTransit;
using Restaurant.Domain.Contracts;

namespace Restaurant.Application.DomainEventHandlers;

public sealed class CreateMenuItemDomainEventHandler(
    IPublishEndpoint publishEndpoint)
    : IDomainEventHandler<CreateMenuItemDomainEvent>
{
    public async Task HandleAsync(CreateMenuItemDomainEvent domainEvent,
        CancellationToken cancellationToken)
    {
        var integrationEvent = new CreateMenuItemIntegrationEvent
        {
            MenuItemName = domainEvent.MenuItemName,
            MenuItemDescription = domainEvent.MenuItemDescription,
            ImageUrl = domainEvent.ImageUrl,
            MenuItemPrice = domainEvent.MenuItemPrice
        };

        await publishEndpoint.Publish(integrationEvent, cancellationToken);
    }
}