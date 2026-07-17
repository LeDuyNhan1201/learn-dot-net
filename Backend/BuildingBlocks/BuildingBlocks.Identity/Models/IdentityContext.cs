namespace BuildingBlocks.Identity.Models;

public sealed class IdentityContext
{
    public required string UserId { get; init; }

    public required string Username { get; init; }

    public string? Email { get; init; }
}