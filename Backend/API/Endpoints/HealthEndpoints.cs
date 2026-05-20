using Infrastructure.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace API.Endpoints;

public static class HealthEndpoints
{
    public static RouteGroupBuilder MapHealthEndpointsV1(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/health").WithTags("Health").WithDisplayName("Health APIs");

        group.MapGet("/info", (IOptions<ServerOptions> options) => options.Value);
        
        group.MapGet("/hello", (string name, [FromServices] IStringLocalizer<Messages> localizer) => localizer["Hello", name].Value);

        return group;
    }
    
    public static RouteGroupBuilder MapHealthEndpointsV2(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/health").WithTags("Health").WithDisplayName("Health APIs");

        group.MapGet("/info", (IOptions<ServerOptions> options) => options.Value);
        
        group.MapGet("/hello", (string name, [FromServices] IStringLocalizer<Messages> localizer) => localizer["Hello", name].Value);
        
        group.MapGet("/test-log", (ILogger<Program> logger) =>
        {
            logger.LogInformation("HELLO FROM OTEL LOGGING");
            return "OK";
        });

        return group;
    }
}