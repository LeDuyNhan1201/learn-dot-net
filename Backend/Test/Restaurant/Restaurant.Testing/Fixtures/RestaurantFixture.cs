using BuildingBlocks.Testing.Fixtures;
using Restaurant.Testing.Factories;

namespace Restaurant.Testing.Fixtures;

public sealed class RestaurantFixture : BaseTestFixture<RestaurantTestFactory>
{
    protected override RestaurantTestFactory CreateFactory()
    {
        return new RestaurantTestFactory(Postgres);
    }

    public override async ValueTask InitializeAsync()
    {
        await base.InitializeAsync();
        await Factory.InitializeDatabaseAsync();
        await Factory.InitializeKeycloakUsersAsync();
    }
}