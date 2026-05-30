using BuildingBlocks.API.Interfaces;
using BuildingBlocks.API.Middlewares;
using BuildingBlocks.Infrastructure.Observability;
using BuildingBlocks.Infrastructure.OpenApi.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.API.Extensions;

public static class ApplicationBuilderExtensions
{
    public static WebApplication UseApplicationPipeline(this WebApplication app, params IEndpointModule[] modules)
    {
        ArgumentNullException.ThrowIfNull(app);

        app
            .UseAppLocalization()
            .UseMetricsExporter(app.Configuration);

        app.UsePathBase(app.Configuration["Server:BasePath"]);

        var versions = modules.Select(x => x.Version).Distinct();
        foreach (var version in versions)
        {
            var group = app.MapGroup(version).WithTags(version);
            foreach (var module in modules.Where(x => x.Version == version))
            {
                module.MapEndpoints(group);
            }
        }

        app.UseScalarUi(app.Configuration);
        
        app
            .UseAuthentication()
            .UseMiddleware<IdentityContextMiddleware>()
            .UseAuthorization();

        return app;
    }
}