using Testcontainers.PostgreSql;

namespace BuildingBlocks.Testing.Fixtures;

public sealed class PostgreSqlFixture : IAsyncLifetime
{
    private const string PostgreSqlImage = "postgres:15.18-alpine";
    private readonly PostgreSqlContainer _container =
        new PostgreSqlBuilder(PostgreSqlImage)
            .WithDatabase("restaurant")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

    public string ConnectionString => _container.GetConnectionString();

    public async ValueTask InitializeAsync()
    {
        await _container.StartAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await _container.DisposeAsync();
    }
}