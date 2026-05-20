using API.Endpoints;
using Infrastructure.Observability;
using Infrastructure.OpenApi;

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
        
        var apiV1 = app
            .MapGroup(Version1DocumentTransfomer.Version1)
            .WithTags(Version1DocumentTransfomer.Version1);

        apiV1.MapTodoEndpoints();
        apiV1.MapHealthEndpointsV1();
        
        var apiV2 = app
            .MapGroup(Version2DocumentTransfomer.Version2)
            .WithTags(Version2DocumentTransfomer.Version2);

        apiV2.MapHealthEndpointsV2();
        
        app.UseOpenApiUi(app.Configuration);

        return app;
    }
    
    private static RouteGroupBuilder GroupEndpointsV1(this WebApplication app)
    {
        var apiV1 = app
            .MapGroup(Version1DocumentTransfomer.Version1)
            .WithTags(Version1DocumentTransfomer.Version1);

        apiV1.MapTodoEndpoints();
        apiV1.MapHealthEndpointsV1();

        return apiV1;
    }
    
    private static RouteGroupBuilder GroupEndpointsV2(this WebApplication app)
    {
        var apiV2 = app
            .MapGroup(Version2DocumentTransfomer.Version2)
            .WithTags(Version2DocumentTransfomer.Version2);

        apiV2.MapHealthEndpointsV2();

        return apiV2;
    }
}