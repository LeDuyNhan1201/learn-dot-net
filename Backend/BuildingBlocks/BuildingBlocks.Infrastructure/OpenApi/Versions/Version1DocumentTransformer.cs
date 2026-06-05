using BuildingBlocks.Infrastructure.OpenApi.Extensions;
using BuildingBlocks.Infrastructure.OpenApi.Options;
using BuildingBlocks.Infrastructure.OpenApi.Utils;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;

namespace BuildingBlocks.Infrastructure.OpenApi.Versions;

public class Version1DocumentTransformer(
    IOptions<ApiDocsOptions> apiDocsOptions,
    ILogger<Version1DocumentTransformer> logger) : IOpenApiDocumentTransformer
{
    public const string Version1 = "v1";

    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        document.InitBaseDocument(apiDocsOptions.Value, Version1);
        foreach (var path in document.Paths) LogMessages.OpenApiPath(logger, path.Key);

        return Task.CompletedTask;
    }
}