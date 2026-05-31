using BuildingBlocks.Infrastructure.OpenApi.Options;
using BuildingBlocks.Infrastructure.OpenApi.Versions;
using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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

    public static IApplicationBuilder UseScalarUi(this WebApplication app, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(app);
        ArgumentNullException.ThrowIfNull(configuration);

        if (app.Environment.IsProduction()) return app;

        var apiDocsOptions = configuration.GetSection(ApiDocsOptions.SectionName).Get<ApiDocsOptions>()
                             ?? throw new InvalidOperationException("OpenAPI configuration is missing.");

        var authOptions = configuration.GetSection(KeycloakAuthenticationOptions.Section).Get<KeycloakAuthenticationOptions>()
                          ?? throw new InvalidOperationException("Authentication configuration is missing.");

        var apiDocsRoute = $"/{apiDocsOptions.ApiDocs}/{{documentName}}.json";

        app.MapOpenApi(apiDocsRoute);

        app.MapScalarApiReference("/docs", scalarOptions =>
        {
            scalarOptions.WithOpenApiRoutePattern(apiDocsRoute);

            foreach (var document in Documents) scalarOptions.AddDocument(document.Version, $"{apiDocsOptions.Title} {document.Version}");

            scalarOptions.WithTitle(apiDocsOptions.Title ?? "APIs Documentation");

            scalarOptions.AddAuthorizationCodeFlow(SecuritySchemeType.OAuth2.GetDisplayName(), flow =>
            {
                flow.ClientId = authOptions.Resource;

                flow.AuthorizationUrl = authOptions.AuthServerUrl;

                flow.TokenUrl = $"{authOptions.AuthServerUrl}{KeycloakConstants.TokenEndpointPath}";

                flow.RedirectUri = $"{apiDocsOptions.ServerUrl}/docs";

                flow.RefreshUrl = flow.TokenUrl;

                flow.SelectedScopes = ["openid", "profile", "email"];

                flow.CredentialsLocation = CredentialsLocation.Header;

                flow.Pkce = Pkce.Sha256;
            });
        });

        app.UseDeveloperExceptionPage();

        return app;
    }
}