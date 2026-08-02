using MassTransit;
using Microsoft.Extensions.Logging;
using Restaurant.Domain.Contracts.IntegrationEvents;

namespace Restaurant.Application.Consumers;

public sealed class MenuItemCreatedConsumer(ILogger<MenuItemCreatedConsumer> logger)
    : IConsumer<MenuItemCreatedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<MenuItemCreatedIntegrationEvent> context)
    {
        logger.LogInformation("Received menu item: {Name}", context.Message.MenuItemName);
        await Task.CompletedTask;
    }
}