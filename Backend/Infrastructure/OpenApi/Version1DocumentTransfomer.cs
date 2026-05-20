using Infrastructure.Options;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;

namespace Infrastructure.OpenApi;

public class Version1DocumentTransfomer
    (IOptions<ApiDocsOptions> apiDocsOptions) : IOpenApiDocumentTransformer
{
    private readonly ApiDocsOptions _apiDocsOptions = apiDocsOptions.Value;
    
    public const string Version1 = "v1";

    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        OpenApiPaths filteredPaths = [];
        document.Paths
            .Where(path =>
                path.Key.StartsWith($"/{Version1}", StringComparison.OrdinalIgnoreCase))
            .ToList()
            .ForEach(path => filteredPaths.Add(path.Key, path.Value));

        document.Paths = filteredPaths;

        Console.WriteLine($"Filtered OpenAPI document paths for version '{Version1}':");
        foreach (var path in document.Paths)        {
            Console.WriteLine($"- {path.Key}");
        }
        
        document.Components ??= new OpenApiComponents();

        document.Servers ??= new List<OpenApiServer>();

        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();

        document.Security ??= new List<OpenApiSecurityRequirement>();

        document.Info = new OpenApiInfo
        {
            Title = _apiDocsOptions.Title,
            Version = _apiDocsOptions.Version,
            Description = _apiDocsOptions.Description,
            TermsOfService = new Uri("https://openapi.io/terms/"),
            Contact = new OpenApiContact
            {
                Name = _apiDocsOptions.ContactName,
                Email = _apiDocsOptions.ContactEmail
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
            Url = _apiDocsOptions.ServerUrl,
            Description = _apiDocsOptions.Description
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
        
        return Task.CompletedTask;
    }
}