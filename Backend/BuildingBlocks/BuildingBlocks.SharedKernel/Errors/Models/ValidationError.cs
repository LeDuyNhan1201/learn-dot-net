namespace BuildingBlocks.SharedKernel.Errors.Models;

public sealed record ValidationError(
    string Field,
    string MessageKey,
    object[] Arguments);