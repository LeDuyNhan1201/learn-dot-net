using BuildingBlocks.Application.Options;
using BuildingBlocks.Infrastructure.OpenApi.Operations;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace BuildingBlocks.Infrastructure.OpenApi.Extensions;

public static class ApiDocumentExtensions
{
    public static OpenApiDocument InitBaseDocument(this OpenApiDocument document, ApiDocsOptions options, string version)
    {
        OpenApiPaths filteredPaths = [];
        document.Paths
            .Where(path => path.Key.StartsWith($"/{version}", StringComparison.OrdinalIgnoreCase))
            .ToList()
            .ForEach(path => filteredPaths.Add(path.Key, path.Value));

        document.Paths = filteredPaths;
        
        document.Components ??= new OpenApiComponents();

        document.Servers ??= new List<OpenApiServer>();

        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();

        document.Security ??= new List<OpenApiSecurityRequirement>();

        document.Info = new OpenApiInfo
        {
            Title = options.Title,
            Version = options.Version,
            Description = options.Description,
            TermsOfService = new Uri("https://openapi.io/terms/"),
            Contact = new OpenApiContact
            {
                Name = options.ContactName,
                Email = options.ContactEmail
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
            Url = options.ServerUrl,
            Description = options.Description
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

        document.Components.SecuritySchemes[SecuritySchemeType.OAuth2.GetDisplayName()] =
            new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri("https://example.com/auth"),
                        TokenUrl = new Uri("https://example.com/token"),
                        Scopes = new Dictionary<string, string>
                        {
                            { "read", "Read access" },
                            { "write", "Write access" }
                        }
                    }
                }
            };

        document.Components.SecuritySchemes[SecuritySchemeType.OpenIdConnect.GetDisplayName()] =
            new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OpenIdConnect,
                OpenIdConnectUrl = new Uri("https://your-keycloak.com/realms/your-realm/.well-known/openid-configuration")
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