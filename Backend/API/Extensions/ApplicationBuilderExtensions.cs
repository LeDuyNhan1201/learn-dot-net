using API.Configurations;
using API.Endpoints;

namespace API.Extensions;

internal static class HostingExtensions
{
    public static WebApplication UseApplicationPipeline(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        app.UseAppLocalization();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.MapTodoEndpoints();
        app.MapHealthEndpoints();

        return app;
    }
}