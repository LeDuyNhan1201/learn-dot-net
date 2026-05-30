namespace BuildingBlocks.Shared.DTOs;

public sealed record ErrorResponse(
    string Type,
    string Message);