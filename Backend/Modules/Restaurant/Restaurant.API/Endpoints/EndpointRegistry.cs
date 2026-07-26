using BuildingBlocks.API.Interfaces;

namespace Restaurant.API.Endpoints;

public static class EndpointRegistry
{
    public static readonly IEndpointModule[] All =
    [
        new v1.HealthEndpoints(),
        new v1.TodoEndpoints(),
        new v1.MenuItemEndpoints(),
        
        new v2.HealthEndpoints(),
        new v2.MenuItemEndpoints()
    ];
}