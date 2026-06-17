namespace BuildingBlocks.Shared.Errors.Models;

public sealed record ValidationError(
    string Field,
    string MessageKey,
    object[] Arguments);