using API.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace API.Endpoints;

public static class HealthEndpoints
{
    public static RouteGroupBuilder MapHealthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/health").WithTags("Health").WithDisplayName("Health APIs");

        group.MapGet("/info", (IOptions<AppOptions> options) => options.Value);
        
        group.MapGet("/hello", (string name, [FromServices] IStringLocalizer<Messages> localizer) => localizer["Hello", name].Value);

        return group;
    }
}