namespace BuildingBlocks.SharedKernel.DTOs;

public record CustomValidationResponse : BaseResponse
{
    public IDictionary<string, string[]>? Errors { get; init; }
    public IEnumerable<string>? OtherErrors { get; init; }
}