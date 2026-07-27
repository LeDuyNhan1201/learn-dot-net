using System.Text.Json.Serialization;

namespace BuildingBlocks.SharedKernel.DTOs;

public record BaseResponse
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Code { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Message { get; set; }
}

public sealed record BaseResponse<T> : BaseResponse
{
    public T? Data { get; init; }
}