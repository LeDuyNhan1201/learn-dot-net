using BuildingBlocks.OpenApi.Extensions;
using BuildingBlocks.OpenApi.Options;
using BuildingBlocks.OpenApi.Utils;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;

namespace BuildingBlocks.OpenApi.Versions;

public class Version2DocumentTransformer(
    IOptions<ApiDocsOptions> apiDocsOptions,
    ILogger<Version2DocumentTransformer> logger) : IOpenApiDocumentTransformer
{
    public const string Version2 = "v2";

    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        document.InitBaseDocument(apiDocsOptions.Value, Version2);
        foreach (var path in document.Paths) LogMessages.OpenApiPath(logger, path.Key);

        return Task.CompletedTask;
    }
}