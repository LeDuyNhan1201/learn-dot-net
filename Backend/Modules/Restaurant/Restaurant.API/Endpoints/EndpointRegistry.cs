using BuildingBlocks.API.Interfaces;

namespace Restaurant.API.Endpoints;

public static class EndpointRegistry
{
    public static readonly IEndpointModule[] All =
    [
        new HealthEndpointsV1(),
        new HealthEndpointsV2(),
        new TodoEndpointsV1(),
        new MenuItemEndpointsV1(),
        new MenuItemEndpointsV2()
    ];
}