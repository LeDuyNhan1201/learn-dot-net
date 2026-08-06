using BuildingBlocks.Testing.Fixtures;
using BuildingBlocks.Testing.Messaging;
using BuildingBlocks.Testing.PostgreSQL;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Restaurant.Application.Consumers;
using Restaurant.Infrastructure.Persistence;

namespace Restaurant.Testing.Factories;

public sealed class RestaurantTestFactory(
    PostgreSqlFixture postgres
    ) : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureServices(services =>
        {
            services.ConfigureTestPostgres<RestaurantDbContext>(postgres);
            services.ConfigureTestMassTransit<RestaurantDbContext>(typeof(MenuItemCreatedConsumer));
            // services.ConfigureTestJwtAuthentication();
        });
    }

    public async Task InitializeDatabaseAsync()
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<RestaurantDbContext>();
        await db.Database.MigrateAsync();
    }
}