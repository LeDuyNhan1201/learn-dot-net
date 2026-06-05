using BuildingBlocks.Infrastructure.OpenApi.Options;
using BuildingBlocks.Infrastructure.OpenApi.Utils;
using BuildingBlocks.Infrastructure.OpenApi.Versions;
using Keycloak.AuthServices.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
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

        foreach (var document in Documents) services.AddOpenApi(document.Version, options => document.Configure(options));

        services.AddEndpointsApiExplorer();

        return services;
    }

    public static IApplicationBuilder UseScalarUi(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        if (app.Environment.IsProduction()) return app;

        var apiDocsOptions = app.Services.GetRequiredService<IOptions<ApiDocsOptions>>().Value;
        var authorizationEndpoint = KeycloakEndpointUrls.GetAuthorizationEndpoint(apiDocsOptions.Keycloak).ToString();
        var tokenEndpoint = KeycloakEndpointUrls.GetTokenEndpoint(apiDocsOptions.Keycloak).ToString();

        var apiDocsRoute = $"/{apiDocsOptions.ApiDocs}/{{documentName}}.json";

        app.MapOpenApi(apiDocsRoute);

        app.MapScalarApiReference("/docs", scalarOptions =>
        {
            scalarOptions.WithOpenApiRoutePattern(apiDocsRoute);

            foreach (var document in Documents) scalarOptions.AddDocument(document.Version, $"{apiDocsOptions.Title} {document.Version}");

            scalarOptions.WithTitle(apiDocsOptions.Title ?? "APIs Documentation");

            // scalarOptions.AddAuthorizationCodeFlow(SecuritySchemeType.OAuth2.GetDisplayName(), flow =>
            // {
            //     flow.ClientId = apiDocsOptions.Keycloak.ClientId;
            //
            //     flow.AuthorizationUrl = authorizationEndpoint;
            //
            //     flow.TokenUrl = tokenEndpoint;
            //
            //     flow.RedirectUri = $"{apiDocsOptions.ServerUrl}/docs";
            //
            //     flow.RefreshUrl = flow.TokenUrl;
            //
            //     flow.SelectedScopes = ["openid", "profile", "email"];
            //
            //     flow.CredentialsLocation = CredentialsLocation.Header;
            //
            //     flow.Pkce = Pkce.Sha256;
            // });
        });

        app.UseDeveloperExceptionPage();

        return app;
    }
}
