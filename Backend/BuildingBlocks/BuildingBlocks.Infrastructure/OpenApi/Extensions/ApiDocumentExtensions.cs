using BuildingBlocks.Infrastructure.OpenApi.Operations;
using BuildingBlocks.Infrastructure.OpenApi.Options;
using BuildingBlocks.Infrastructure.OpenApi.Utils;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace BuildingBlocks.Infrastructure.OpenApi.Extensions;

public static class ApiDocumentExtensions
{
    public static OpenApiDocument InitBaseDocument(
        this OpenApiDocument document,
        ApiDocsOptions apiDocsOptions,
        string version)
    {
        OpenApiPaths filteredPaths = [];
        document.Paths
            .Where(path => path.Key.StartsWith($"/{version}", StringComparison.OrdinalIgnoreCase))
            .ToList()
            .ForEach(path => filteredPaths.Add(path.Key, path.Value));
        
        document.Paths
            .Where(path => path.Key.StartsWith("/sessions", StringComparison.OrdinalIgnoreCase))
            .ToList()
            .ForEach(path => filteredPaths.Add(path.Key, path.Value));

        document.Paths = filteredPaths;

        document.Components ??= new OpenApiComponents();

        document.Servers ??= new List<OpenApiServer>();

        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();

        document.Security ??= new List<OpenApiSecurityRequirement>();

        document.Info = new OpenApiInfo
        {
            Title = apiDocsOptions.Title,
            Version = apiDocsOptions.Version,
            Description = apiDocsOptions.Description,
            TermsOfService = new Uri("https://openapi.io/terms/"),
            Contact = new OpenApiContact
            {
                Name = apiDocsOptions.ContactName,
                Email = apiDocsOptions.ContactEmail
            },
            License = new OpenApiLicense
            {
                Name = "Apache 2.0",
                Url = new Uri("https://www.apache.org/licenses/LICENSE-2.0.html")
            }
        };

        var defaultServerUrl = document.Servers.FirstOrDefault();
        document.Servers.Clear();
        document.Servers.Add(new OpenApiServer
        {
            Url = apiDocsOptions.ServerUrl,
            Description = apiDocsOptions.Description
        });
        if (defaultServerUrl != null) document.Servers.Add(defaultServerUrl);

        document.Components.SecuritySchemes[SecuritySchemeType.Http.GetDisplayName()] =
            new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Name = "Authorization",
                Description = "JWT Bearer token"
            };

        var authorizationEndpoint = KeycloakEndpointUrls.GetAuthorizationEndpoint(apiDocsOptions.Keycloak);
        var tokenEndpoint = KeycloakEndpointUrls.GetTokenEndpoint(apiDocsOptions.Keycloak);
        var openIdConnectEndpoint = KeycloakEndpointUrls.GetOpenIdConfigurationEndpoint(apiDocsOptions.Keycloak);

        // document.Components.SecuritySchemes[SecuritySchemeType.OAuth2.GetDisplayName()] =
        //     new OpenApiSecurityScheme
        //     {
        //         Type = SecuritySchemeType.OAuth2,
        //         Flows = new OpenApiOAuthFlows
        //         {
        //             AuthorizationCode = new OpenApiOAuthFlow
        //             {
        //                 AuthorizationUrl = authorizationEndpoint,
        //                 TokenUrl = tokenEndpoint,
        //                 RefreshUrl = tokenEndpoint,
        //                 Scopes = new Dictionary<string, string>
        //                 {
        //                     { "openid", "Open ID" },
        //                     { "profile", "Profile access" },
        //                     { "email", "Email access" }
        //                 }
        //             }
        //         }
        //     };

        document.Components.SecuritySchemes[SecuritySchemeType.OpenIdConnect.GetDisplayName()] =
            new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OpenIdConnect,
                OpenIdConnectUrl = openIdConnectEndpoint
            };

        return document;
    }

    public static OpenApiOptions AddCommonTransformers(this OpenApiOptions options)
    {
        return options
            .AddOperationTransformer<LanguageOperationTransformer>()
            .AddOperationTransformer<MultiPartFileOperationTransformer>()
            .AddOperationTransformer<AuthOperationTransformer>();
    }
}