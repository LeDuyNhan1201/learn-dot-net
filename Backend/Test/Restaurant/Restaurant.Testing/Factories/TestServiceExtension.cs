using BuildingBlocks.Testing.Fixtures;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Restaurant.Application.Consumers;
using Restaurant.Infrastructure.Persistence;

namespace Restaurant.Testing.Factories;

public static class TestServiceExtension
{
    public static void ConfigureTestPostgres(this IServiceCollection services, PostgreSqlFixture postgres)
    {
        services.RemoveAll<DbContextOptions<RestaurantDbContext>>();
        services.AddDbContext<RestaurantDbContext>(options => { options.UseNpgsql(postgres.ConnectionString); });
    }

    public static void ConfigureTestMassTransit(this IServiceCollection services)
    {
        services.AddMassTransitTestHarness(busConfig =>
        {
            busConfig.AddConsumer<MenuItemCreatedConsumer>();

            busConfig.SetKebabCaseEndpointNameFormatter();

            busConfig.AddEntityFrameworkOutbox<RestaurantDbContext>(configurator =>
            {
                configurator.UsePostgres();
                configurator.UseBusOutbox();
            });

            busConfig.AddConfigureEndpointsCallback((context, name, endpoint) =>
            {
                endpoint.UseEntityFrameworkOutbox<RestaurantDbContext>(context);
            });

            busConfig.UsingInMemory((context, bus) =>
            {
                bus.ConfigureEndpoints(context);
            });
        });
    }
}