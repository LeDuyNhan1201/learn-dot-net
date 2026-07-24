using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Messaging.Configurations;

public static class InMemoryMassTransitConfiguration
{
    public static IServiceCollection ConfigureInMemory<T>(
        this IServiceCollection services, 
        params Type[] consumerTypes)
    where T : DbContext
    {
        services.AddMassTransit(busConfig =>
        {
            busConfig.SetKebabCaseEndpointNameFormatter();

            busConfig.AddEntityFrameworkOutbox<T>(configurator =>
            {
                configurator.UsePostgres();

                configurator.UseBusOutbox();
            });
            
            foreach (var consumerType in consumerTypes)
                busConfig.AddConsumer(consumerType);

            busConfig.UsingInMemory((context, cfg) =>
            {
                cfg.ConfigureEndpoints(context);
            });
        });
        
        return services;
    }
}