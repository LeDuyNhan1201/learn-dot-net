using BuildingBlocks.API.Interfaces;
using Restaurant.API.Endpoints.v1;

namespace Restaurant.API.Endpoints;

public static class EndpointRegistry
{
    public static readonly IEndpointModule[] All =
    [
        new HealthEndpoints(),
        new TodoEndpoints(),
        new MenuItemEndpoints(),

        new v2.HealthEndpoints(),
        new v2.MenuItemEndpoints()
    ];
}