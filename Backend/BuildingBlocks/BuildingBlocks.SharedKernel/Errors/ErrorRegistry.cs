using BuildingBlocks.SharedKernel.Errors.Models;

namespace BuildingBlocks.SharedKernel.Errors;

public static class ErrorRegistry
{
    private static readonly Dictionary<string, ErrorDefinition> Errors = new()
    {
        [ValidationErrors.Required.Code] = ValidationErrors.Required,
        [ValidationErrors.Email.Code] = ValidationErrors.Email,
        [ValidationErrors.Min.Code] = ValidationErrors.Min,
        [ValidationErrors.Max.Code] = ValidationErrors.Max,
        [ValidationErrors.Range.Code] = ValidationErrors.Range
    };

    public static ErrorDefinition Get(string code)
    {
        return Errors.TryGetValue(code, out var error)
            ? error
            : throw new InvalidOperationException($"Unknown error code '{code}'.");
    }
}