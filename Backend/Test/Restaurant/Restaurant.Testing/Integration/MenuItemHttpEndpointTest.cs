using BuildingBlocks.Testing.Fixtures;
using BuildingBlocks.Testing.Integration;
using Restaurant.Application.DTOs;
using Restaurant.Testing.Factories;

namespace Restaurant.Testing.Integration;

public class MenuItemHttpEndpointTest(PostgreSqlFixture postgres) : HttpEndpointTest, IDisposable
{
    protected const string MenuItemUri = "/restaurant/api/v2/menu-items";
    private readonly RestaurantApiFactory _factory = new(postgres);
    private bool _disposed;
    protected override HttpClient Client => _factory.CreateClient();
    protected override IServiceProvider Services => _factory.Services;
    protected static CreateMenuItemRequest ValidRequest => MenuItemFactory.ValidCreateRequest;
    protected static CreateMenuItemRequest InvalidPriceRequest => MenuItemFactory.InvalidPriceRequest();

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing) _factory.Dispose();

        _disposed = true;
    }
}