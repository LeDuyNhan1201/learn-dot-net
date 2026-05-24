using BuildingBlocks.Infrastructure.OpenApi.Operations;
using BuildingBlocks.Infrastructure.OpenApi.Versions;
using Infrastructure.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scalar.AspNetCore;

namespace BuildingBlocks.Infrastructure.OpenApi;

public static class OpenApiExtensions
{
    public static IServiceCollection AddOpenAPI(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddOpenApi(Version1DocumentTransfomer.Version1, openApiOptions =>
        {
            openApiOptions
                .AddOperationTransformer<LanguageOperationTransformer>()
                .AddOperationTransformer<MultiPartFileOperationTransformer>()
                .AddOperationTransformer<AuthOperationTransformer>()
                .AddDocumentTransformer<Version1DocumentTransfomer>();
        });
        
        services.AddOpenApi(Version2DocumentTransfomer.Version2, openApiOptions =>
        {
            openApiOptions
                .AddOperationTransformer<LanguageOperationTransformer>()
                .AddOperationTransformer<MultiPartFileOperationTransformer>()
                .AddOperationTransformer<AuthOperationTransformer>()
                .AddDocumentTransformer<Version2DocumentTransfomer>();
        });
        
        services.AddEndpointsApiExplorer();
        return services;
    }

    public static IApplicationBuilder UseOpenApiUi(this WebApplication app, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(app);
        ArgumentNullException.ThrowIfNull(configuration);

        if (app.Environment.IsProduction()) return app;

        var options = configuration.GetSection(ApiDocsOptions.SectionName).Get<ApiDocsOptions>()
                      ?? throw new InvalidOperationException("Open API configuration is missing.");

        var apiDocsRoute = $"/{options.ApiDocs}/{{documentName}}.json";
        app.MapOpenApi(apiDocsRoute);
        app.MapScalarApiReference("/docs", scalarOptions =>
        {
            scalarOptions
                .WithOpenApiRoutePattern(apiDocsRoute)
                .AddDocument(Version1DocumentTransfomer.Version1, $"{options.Title} {Version1DocumentTransfomer.Version1}")
                .AddDocument(Version2DocumentTransfomer.Version2, $"{options.Title} {Version2DocumentTransfomer.Version2}");
        });
        app.UseDeveloperExceptionPage();
        return app;
    }
}