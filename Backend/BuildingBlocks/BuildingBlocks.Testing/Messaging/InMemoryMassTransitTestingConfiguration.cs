using BuildingBlocks.Domain.Abstractions.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Testing.Messaging;

public static class InMemoryMassTransitTestingConfiguration
{
    public static void ConfigureTestMassTransit<T>(this IServiceCollection services,
        params Type[] consumerTypes)
        where T : DbContext, IApplicationDbContext
    {
        services.AddMassTransitTestHarness(busConfig =>
        {
            busConfig.SetKebabCaseEndpointNameFormatter();

            // busConfig.AddEntityFrameworkOutbox<T>(configurator =>
            // {
            //     configurator.UsePostgres();
            //     configurator.UseBusOutbox();
            // });

            foreach (var consumerType in consumerTypes) busConfig.AddConsumer(consumerType);

            // busConfig.AddConfigureEndpointsCallback((context, name, endpoint) => { endpoint.UseEntityFrameworkOutbox<T>(context); });

            busConfig.UsingInMemory((context, bus) => { bus.ConfigureEndpoints(context); });
        });
    }
}