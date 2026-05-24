using BuildingBlocks.API.Interfaces;
using BuildingBlocks.Infrastructure.OpenApi.Versions;
using BuildingBlocks.Shared;
using Infrastructure.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace API.Endpoints;

public sealed class HealthEndpointsV1 : IEndpointModule
{
    public string Version => Version1DocumentTransfomer.Version1;
    
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/health").WithTags("Health").WithDisplayName("Health APIs");

        group.MapGet("/info", (IOptions<ServerOptions> options) => options.Value);
        
        group.MapGet("/hello", (string name, [FromServices] CompositeLocalizer<Messages> localizer) => localizer["Hello", name]);
    }
}