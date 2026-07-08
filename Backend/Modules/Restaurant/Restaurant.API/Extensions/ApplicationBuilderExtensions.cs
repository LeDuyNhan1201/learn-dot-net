using BuildingBlocks.API.Extensions;
using BuildingBlocks.API.Middlewares;
using BuildingBlocks.Infrastructure.Observability.Extensions;
using BuildingBlocks.Infrastructure.Persistence.Extensions;
using Restaurant.API.Endpoints;
using Restaurant.Infrastructure.Persistence;

namespace Restaurant.API.Extensions;

/// <summary>
///     Extension methods for configuring the application's request pipeline and initialization.
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    ///     Initializes the application by performing automatic database migrations for the specified DbContext.
    /// </summary>
    /// <param name="app">The <see cref="WebApplication" /> instance to initialize.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public static async Task InitializeAsync(this WebApplication app)
    {
        await app.AutoMigration<RestaurantDbContext>();
    }

    /// <summary>
    ///     Configures the application's request pipeline, including middleware, authentication, authorization, and routing.
    /// </summary>
    /// <param name="app">The <see cref="WebApplication" /> instance to initialize.</param>
    /// <returns>>The configured <see cref="WebApplication" /> instance.</returns>
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