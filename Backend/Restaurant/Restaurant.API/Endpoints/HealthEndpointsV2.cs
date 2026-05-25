using BuildingBlocks.API.Interfaces;
using BuildingBlocks.Application.Options;
using BuildingBlocks.Infrastructure.OpenApi.Versions;
using BuildingBlocks.Shared.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Restaurant.API.Endpoints;

public sealed class HealthEndpointsV2 : IEndpointModule
{
    public string Version => Version2DocumentTransformer.Version2;
    
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/health").WithTags("Health").WithDisplayName("Health APIs");

        group.MapGet("/info", (IOptions<ServerOptions> options) => options.Value);
        
        group.MapGet("/hello", (string name, [FromServices] CompositeLocalizer<Messages> localizer) => localizer["Create", name]);
        
        group.MapGet("/test-log", (ILogger<Program> logger) =>
        {
            logger.LogInformation("HELLO FROM OTEL LOGGING");
            return "OK";
        });
    }
}