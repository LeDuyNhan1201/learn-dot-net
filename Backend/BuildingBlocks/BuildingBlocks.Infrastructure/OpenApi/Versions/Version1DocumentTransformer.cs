using BuildingBlocks.Application.Options;
using BuildingBlocks.Infrastructure.OpenApi.Extensions;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;

namespace BuildingBlocks.Infrastructure.OpenApi.Versions;

public class Version1DocumentTransformer (
    IOptions<ApiDocsOptions> apiDocsOptions, 
    ILogger<Version1DocumentTransformer> logger) : IOpenApiDocumentTransformer
{
    private readonly ApiDocsOptions _apiDocsOptions = apiDocsOptions.Value;
    
    public const string Version1 = "v1";

    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        document.InitBaseDocument(_apiDocsOptions, Version1);
        foreach (var path in document.Paths) Core.LogMessages.OpenApiPath(logger, path.Key);
        
        return Task.CompletedTask;
    }
}