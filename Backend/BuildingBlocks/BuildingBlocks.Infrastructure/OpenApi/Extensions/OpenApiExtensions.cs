using BuildingBlocks.Application.Options;
using BuildingBlocks.Infrastructure.OpenApi.Versions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scalar.AspNetCore;

namespace BuildingBlocks.Infrastructure.OpenApi.Extensions;

public static class OpenApiExtensions
{
    private static readonly OpenApiDocumentDefinition[] Documents =
    [
        new(
            Version1DocumentTransformer.Version1,
            options => options
                .AddCommonTransformers()
                .AddDocumentTransformer<Version1DocumentTransformer>()),

        new(
            Version2DocumentTransformer.Version2,
            options => options
                .AddCommonTransformers()
                .AddDocumentTransformer<Version2DocumentTransformer>())
    ];
    
    public static IServiceCollection AddScalarOpenApi(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        foreach (var document in Documents)
        {
            services.AddOpenApi(document.Version, options => document.Configure(options));
        }

        services.AddEndpointsApiExplorer();

        return services;
    }

    public static IApplicationBuilder UseScalarUi(this WebApplication app, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(app);
        ArgumentNullException.ThrowIfNull(configuration);

        if (app.Environment.IsProduction()) return app;

        var options = configuration.GetSection(ApiDocsOptions.SectionName).Get<ApiDocsOptions>() 
                      ?? throw new InvalidOperationException("OpenAPI configuration is missing.");

        var apiDocsRoute = $"/{options.ApiDocs}/{{documentName}}.json";

        app.MapOpenApi(apiDocsRoute);

        app.MapScalarApiReference("/docs", scalarOptions => 
        {
            scalarOptions.WithOpenApiRoutePattern(apiDocsRoute);
            foreach (var document in Documents)
            {
                scalarOptions.AddDocument(document.Version, $"{options.Title} {document.Version}");
            }
        });

        app.UseDeveloperExceptionPage();

        return app;
    }
}