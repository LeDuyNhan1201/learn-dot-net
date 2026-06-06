namespace BuildingBlocks.Shared.DTOs;

public sealed record BaseResponse<T>(
    string Code,
    string Message,
    T? Data = default);