using Microsoft.AspNetCore.Routing;

namespace BuildingBlocks.API.Abstractions;

public interface IEndpointModule
{
    string Version { get; }
    void MapEndpoints(IEndpointRouteBuilder group);
}