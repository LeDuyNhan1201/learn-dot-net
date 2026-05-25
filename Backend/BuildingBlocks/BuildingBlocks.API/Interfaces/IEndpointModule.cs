using Microsoft.AspNetCore.Routing;

namespace BuildingBlocks.API.Interfaces;

public interface IEndpointModule
{
    string Version { get; }
    void MapEndpoints(IEndpointRouteBuilder group);
}