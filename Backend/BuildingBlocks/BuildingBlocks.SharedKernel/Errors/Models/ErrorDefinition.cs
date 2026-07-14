namespace BuildingBlocks.Shared.Errors.Models;

public sealed record ErrorDefinition(
    string Code,
    string MessageKey);