using BuildingBlocks.Domain.Services;
using BuildingBlocks.Testing.Factories;
using BuildingBlocks.Testing.Fixtures;
using BuildingBlocks.Testing.Messaging;
using BuildingBlocks.Testing.PostgreSQL;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Restaurant.Application.Consumers;
using Restaurant.Infrastructure.Persistence;

namespace Restaurant.Testing.Factories;

public sealed class RestaurantTestFactory(PostgreSqlFixture postgres) : BaseTestFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.ConfigureServices(services =>
        {
            services.ConfigureTestPostgres<RestaurantDbContext>(postgres);
            services.ConfigureTestMassTransit<RestaurantDbContext>(typeof(MenuItemCreatedConsumer));
        });
    }

    public async Task InitializeDatabaseAsync()
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<RestaurantDbContext>();
        await db.Database.MigrateAsync();
    }
    
    public async Task InitializeKeycloakUsersAsync()
    {
        using var scope = Services.CreateScope();
        var seeder = scope.ServiceProvider.GetRequiredService<IUserSeederService>();
        await seeder.InitAdministrators();
        await seeder.InitCustomers();
    }
}