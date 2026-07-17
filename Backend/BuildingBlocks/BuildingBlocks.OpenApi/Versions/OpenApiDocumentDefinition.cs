using Microsoft.AspNetCore.OpenApi;

namespace BuildingBlocks.OpenApi.Versions;

public sealed record OpenApiDocumentDefinition(
    string Version,
    Action<OpenApiOptions> Configure);