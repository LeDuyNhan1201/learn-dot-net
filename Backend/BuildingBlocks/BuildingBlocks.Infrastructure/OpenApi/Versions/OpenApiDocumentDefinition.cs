using Microsoft.AspNetCore.OpenApi;

namespace BuildingBlocks.Infrastructure.OpenApi.Versions;

public sealed record OpenApiDocumentDefinition(
    string Version,
    Action<OpenApiOptions> Configure);