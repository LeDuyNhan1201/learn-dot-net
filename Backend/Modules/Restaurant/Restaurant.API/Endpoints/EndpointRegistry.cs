using BuildingBlocks.API.Interfaces;

namespace Restaurant.API.Endpoints;

/// <summary>
///     This class serves as a registry for all endpoint modules in the application.
/// </summary>
public static class EndpointRegistry
{
    /// <summary>
    ///     A collection of all endpoint modules available in the application.
    /// </summary>
    public static readonly IEndpointModule[] All =
    [
        new HealthEndpointsV1(),
        new HealthEndpointsV2(),
        new TodoEndpointsV1(),
        new MenuItemEndpointsV1(),
        new MenuItemEndpointsV2()
    ];
}