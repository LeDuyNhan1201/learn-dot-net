using System.Text.Json.Serialization;

namespace BuildingBlocks.SharedKernel.DTOs;

public record CustomValidationResponse : BaseResponse
{
    [JsonPropertyName("errors")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IDictionary<string, string[]>? Errors { get; init; }

    [JsonPropertyName("otherErrors")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IEnumerable<string>? OtherErrors { get; init; }
}