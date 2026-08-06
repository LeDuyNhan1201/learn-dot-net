using Testcontainers.Keycloak;

namespace BuildingBlocks.Testing.Fixtures;

public sealed class KeycloakFixture : IAsyncLifetime
{
    private readonly KeycloakContainer _container =
        new KeycloakBuilder("quay.io/keycloak/keycloak:21.1")
            .WithRealm(Path.Combine(AppContext.BaseDirectory, "TestData/Keycloak", "learn-dot-net-test-realm.json"))
            .WithUsername("admin")
            .WithPassword("admin")
            .Build();

    public async ValueTask InitializeAsync()
    {
        await _container.StartAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await _container.DisposeAsync();
    }
}