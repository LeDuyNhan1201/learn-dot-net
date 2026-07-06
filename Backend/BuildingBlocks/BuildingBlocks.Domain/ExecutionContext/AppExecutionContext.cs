using BuildingBlocks.Domain.Enumerations;

namespace BuildingBlocks.Domain.ExecutionContext;

public sealed class AppExecutionContext
{
    public static readonly AppExecutionContext System = new()
    {
        ActorType = ActorType.System,
        Source = "Unknown"
    };

    public string? UserId { get; init; }

    public string? Username { get; init; }

    public string? Email { get; init; }

    public string? TenantId { get; init; }

    public string? CorrelationId { get; init; }

    public string? RequestId { get; init; }

    public string? ClientIp { get; init; }

    public string? UserAgent { get; init; }

    public ActorType ActorType { get; init; }

    public string? Source { get; init; }
}