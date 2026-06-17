using System.Text.Json.Serialization;

namespace BuildingBlocks.Shared.DTOs;

public sealed record BaseResponse<T>
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Code { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Message { get; set; }

    public T? Data { get; init; }
}