using API.Endpoints;
using Infrastructure.Observability;
using Infrastructure.Swagger;

namespace API.Extensions;

internal static class ApplicationBuilderExtensions
{
    public static WebApplication UseApplicationPipeline(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        app
            .UseAppLocalization()
            .UseMetricsExporter(app.Configuration);
        
        app.UsePathBase(app.Configuration["Server:BasePath"]);
        
        var apiVersion = app.Configuration["Server:ApiVersion"] ?? "v1";
        var apiV1 = app
            .MapGroup(apiVersion)
            .WithTags(apiVersion);

        apiV1.MapTodoEndpoints();
        apiV1.MapHealthEndpoints();

        app.UseSwaggerUi(app.Configuration);

        return app;
    }
}