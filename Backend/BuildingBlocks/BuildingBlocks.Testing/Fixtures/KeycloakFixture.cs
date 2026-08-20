using Testcontainers.Keycloak;

namespace BuildingBlocks.Testing.Fixtures;

public sealed class KeycloakFixture : IAsyncLifetime
{
    private const string KeycloakImage = "quay.io/keycloak/keycloak:26.6.2";

    private readonly KeycloakContainer _container =
        new KeycloakBuilder(KeycloakImage)
            .WithRealm(Path.Combine(AppContext.BaseDirectory, "TestData/Keycloak", "learn-dot-net-test-realm.json"))
            .WithUsername("admin")
            .WithPassword("admin")
            .WithPortBinding(3333, 8080)
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