using BuildingBlocks.Testing.Fixtures;
using Restaurant.Testing.Factories;
using Xunit;

namespace Restaurant.Testing.Fixtures;

public sealed class RestaurantFixture : IAsyncLifetime
{
    private readonly PostgreSqlFixture _postgres = new();
    
    private readonly KeycloakFixture _keycloak = new();

    public RestaurantTestFactory Factory { get; private set; } = null!;

    public HttpClient Client { get; private set; } = null!;

    public async ValueTask InitializeAsync()
    {
        await _postgres.InitializeAsync();
        
        await _keycloak.InitializeAsync();

        Factory = new RestaurantTestFactory(_postgres);

        await Factory.InitializeDatabaseAsync();

        Client = Factory.CreateClient();
    }

    public async ValueTask DisposeAsync()
    {
        await Factory.DisposeAsync();
        await _postgres.DisposeAsync();
        await _keycloak.DisposeAsync();
    }
}