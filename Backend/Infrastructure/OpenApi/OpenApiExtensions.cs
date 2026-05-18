using Infrastructure.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi;
using Scalar.AspNetCore;

namespace Infrastructure.OpenApi;

public static class OpenApiExtensions
{
    public static IServiceCollection AddOpenAPI(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var options = configuration.GetSection(OpenApiOptions.SectionName).Get<OpenApiOptions>()
                      ?? throw new InvalidOperationException("Open APi configuration is missing.");

        services.AddOpenApi(options.ApiDocs, openApiOptions =>
        {
            openApiOptions.AddOperationTransformer<MultiPartFileOperationTransformer>();
            openApiOptions.AddOperationTransformer<AuthOperationTransformer>();

            openApiOptions.AddDocumentTransformer((document, _, _) =>
            {
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

                document.Servers.Clear();
                document.Servers.Add(new OpenApiServer
                {
                    Url = options.ServerUrl,
                    Description = options.Description
                });

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
            });
        });

        services.AddEndpointsApiExplorer();
        return services;
    }

    public static IApplicationBuilder UseOpenApiUi(this WebApplication app, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(app);
        ArgumentNullException.ThrowIfNull(configuration);

        if (app.Environment.IsProduction()) return app;

        var options = configuration.GetSection(OpenApiOptions.SectionName).Get<OpenApiOptions>()
                      ?? throw new InvalidOperationException("Open API configuration is missing.");

        var apiDocsRoute = "/open-api/" + options.Version + "/{documentName}.json";
        app.MapOpenApi(apiDocsRoute);
        app.MapScalarApiReference("/docs", scalarOptions =>
        {
            scalarOptions
                .WithOpenApiRoutePattern(apiDocsRoute)
                .AddDocument(options.ApiDocs, options.Title);
        });
        app.UseDeveloperExceptionPage();
        return app;
    }
}