using BuildingBlocks.Application.RestClients;
using BuildingBlocks.Domain.Services;
using BuildingBlocks.Testing.Factories;
using BuildingBlocks.Testing.Fixtures;
using BuildingBlocks.Testing.Messaging;
using BuildingBlocks.Testing.PostgreSQL;
using FluentAssertions;
using Keycloak.AuthServices.Sdk.Admin.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Restaurant.Application.Consumers;
using Restaurant.Infrastructure.Persistence;

namespace Restaurant.Testing.Factories;

public sealed class RestaurantTestFactory(PostgreSqlFixture postgres) : BaseTestFactory<Program>
{
    public string AdminAccessToken { get; set; } = string.Empty;
    public string CustomerAccessToken { get; set; } = string.Empty;
    
    public UserRepresentation AdminUser { get; set; } = null!;
    public UserRepresentation CustomerUser { get; set; } = null!;
    
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
    
    public async Task<string> GetAccessTokenAsync(string username, string password)
    {
        using var scope = Services.CreateScope();
        var keycloakAdminClient = scope.ServiceProvider.GetRequiredService<IKeycloakAdminClient>();
        var response = await keycloakAdminClient.GetTokensAsync(username, password);
        return response.AccessToken;
    }
    
    public async Task<UserRepresentation> GetUserByEmailAsync(string email)
    {
        using var scope = Services.CreateScope();
        var keycloakAdminClient = scope.ServiceProvider.GetRequiredService<IKeycloakAdminClient>();
        var user = await keycloakAdminClient.GetUserByEmailAsync(email);
        user.Should().NotBeNull($"User with email {email} should exist in Keycloak.");
        return user;
    }
}