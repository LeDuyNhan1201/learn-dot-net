using BuildingBlocks.API.Interfaces;
using BuildingBlocks.OpenApi.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.API.Endpoints;

public static class EndpointRouteBuilderExtensions
{
    public static WebApplication UseRestRouting(this WebApplication app, params IEndpointModule[] modules)
    {
        ArgumentNullException.ThrowIfNull(app);

        var versions = modules.Select(x => x.Version).Distinct();
        foreach (var version in versions)
        {
            var group = app.MapGroup(version).WithTags(version);
            foreach (var module in modules.Where(x => x.Version == version)) module.MapEndpoints(group);
        }

        app.UseScalarUi();

        return app;
    }
}