using BuildingBlocks.SharedKernel.Helpers;
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
        
        Factory.AdminAccessToken = await Factory.GetAccessTokenAsync(Constants.AdministratorSampleEmail, Constants.AdministratorSamplePassword);
        Factory.CustomerAccessToken = await Factory.GetAccessTokenAsync(Constants.CustomerSampleEmail, Constants.CustomerSamplePassword);
        
        Factory.AdminUser = await Factory.GetUserByEmailAsync(Constants.AdministratorSampleEmail);
        Factory.CustomerUser = await Factory.GetUserByEmailAsync(Constants.CustomerSampleEmail);
    }
}