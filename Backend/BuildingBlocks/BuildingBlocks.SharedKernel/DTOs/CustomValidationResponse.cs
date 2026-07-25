namespace BuildingBlocks.SharedKernel.DTOs;

public sealed record CustomValidationResponse
{
    public string? Code { get; init; }
    public string? Message { get; set; }
    public IDictionary<string, string[]>? Errors { get; init; }
    public IEnumerable<string>? OtherErrors { get; init; }
}