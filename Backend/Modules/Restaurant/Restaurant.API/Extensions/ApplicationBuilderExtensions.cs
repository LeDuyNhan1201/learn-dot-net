using BuildingBlocks.API.Endpoints;
using BuildingBlocks.Identity.Middlewares;
using BuildingBlocks.Observability.Extensions;
using BuildingBlocks.Persistence.Extensions;
using BuildingBlocks.SharedKernel.Localization;
using Restaurant.API.Endpoints;
using Restaurant.Infrastructure.Persistence;

namespace Restaurant.API.Extensions;

public static class ApplicationBuilderExtensions
{
    public static async Task InitializeAsync(this WebApplication app)
    {
        await app.AutoMigration<RestaurantDbContext>();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        var configuration = app.Configuration;

        app.UsePathBase(configuration["Server:BasePath"]);

        app.UseAppLocalization();

        app.UseMetricsExporter(configuration);

        app.UseExceptionHandler();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseMiddleware<IdentityContextMiddleware>();

        app.MapTokenEndpoints();

        app.UseRestRouting(EndpointRegistry.All);

        return app;
    }
}