namespace BuildingBlocks.Shared.Errors.Models;

public static class ValidationErrors
{
    public const string PrefixCode = "error/validation";
    public const string PrefixMessageKey = "Error.Validation";

    public static readonly ErrorDefinition Email = new(
        $"{PrefixCode}/email",
        $"{PrefixMessageKey}.Email"
    );

    public static readonly ErrorDefinition Required = new(
        $"{PrefixCode}/required",
        $"{PrefixMessageKey}.Required"
    );

    public static readonly ErrorDefinition Valid = new(
        $"{PrefixCode}/valid",
        $"{PrefixMessageKey}.Valid"
    );

    public static readonly ErrorDefinition Range = new(
        $"{PrefixCode}/range",
        $"{PrefixMessageKey}.Range"
    );

    public static readonly ErrorDefinition Min = new(
        $"{PrefixCode}/min",
        $"{PrefixMessageKey}.Min"
    );

    public static readonly ErrorDefinition Max = new(
        $"{PrefixCode}/max",
        $"{PrefixMessageKey}.Max"
    );
}