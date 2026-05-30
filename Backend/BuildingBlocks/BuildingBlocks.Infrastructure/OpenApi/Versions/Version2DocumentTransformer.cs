using BuildingBlocks.Application.Options;
using BuildingBlocks.Infrastructure.OpenApi.Extensions;
using Keycloak.AuthServices.Authentication;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;

namespace BuildingBlocks.Infrastructure.OpenApi.Versions;

public class Version2DocumentTransformer(
    IOptions<ApiDocsOptions> apiDocsOptions,
    IOptions<KeycloakAuthenticationOptions> authOptions,
    ILogger<Version1DocumentTransformer> logger) : IOpenApiDocumentTransformer
{
    public const string Version2 = "v2";

    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        document.InitBaseDocument(apiDocsOptions.Value, authOptions.Value, Version2);
        foreach (var path in document.Paths) Core.LogMessages.OpenApiPath(logger, path.Key);
        
        return Task.CompletedTask;
    }
}