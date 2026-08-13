using BuildingBlocks.Testing.Factories;

namespace BuildingBlocks.Testing.Fixtures;

public abstract class BaseTestFixture<TFactory> : IAsyncLifetime
    where TFactory : IBaseTestFactory
{
    protected PostgreSqlFixture Postgres { get; } = new();

    protected KeycloakFixture Keycloak { get; } = new();

    public TFactory Factory { get; private set; } = default!;

    public HttpClient Client { get; private set; } = null!;

    protected abstract TFactory CreateFactory();

    public virtual async ValueTask InitializeAsync()
    {
        await Postgres.InitializeAsync();
        await Keycloak.InitializeAsync();

        Factory = CreateFactory();
        Client = Factory.CreateClient();
    }

    public virtual async ValueTask DisposeAsync()
    {
        Client.Dispose();

        await Factory.DisposeAsync();
        await Postgres.DisposeAsync();
        await Keycloak.DisposeAsync();
    }
}