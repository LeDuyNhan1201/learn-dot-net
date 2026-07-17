namespace BuildingBlocks.SharedKernel.Errors.Models;

public sealed record ErrorDefinition(
    string Code,
    string MessageKey);