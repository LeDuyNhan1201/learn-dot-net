using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Infrastructure.OpenApi.Utils;

public static partial class LogMessages
{
    [LoggerMessage(
        EventId = 1001,
        Level = LogLevel.Debug,
        Message = "OpenAPI Path: {Path}")]
    public static partial void OpenApiPath(ILogger logger, string path);
}