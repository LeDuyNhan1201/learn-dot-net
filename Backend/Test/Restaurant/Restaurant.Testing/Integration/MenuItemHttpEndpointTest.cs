using BuildingBlocks.Testing.Integration;
using Restaurant.Application.DTOs;
using Restaurant.Domain.Contracts.Commands;
using Restaurant.Testing.Factories;
using Restaurant.Testing.Fixtures;

namespace Restaurant.Testing.Integration;

public class MenuItemHttpEndpointTest(RestaurantFixture fixture) : HttpEndpointTest
{
    protected const string MenuItemUri = "/restaurant/api/v2/menu-items";
    protected override HttpClient Client => fixture.Client;
    protected override IServiceProvider Services => fixture.Factory.Services;
    protected static CreateMenuItemRequest ValidRequest => MenuItemFactory.ValidCreateRequest;
    protected static CreateMenuItemRequest InvalidPriceRequest => MenuItemFactory.InvalidPriceCreateRequest;
    protected static CreateMenuItemCommand ValidCommand => MenuItemFactory.ValidCreateCommand;
    protected static CreateMenuItemCommand InvalidPriceCommand => MenuItemFactory.InvalidPriceCreateCommand;
}