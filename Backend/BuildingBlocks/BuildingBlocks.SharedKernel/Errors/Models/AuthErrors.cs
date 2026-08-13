namespace BuildingBlocks.SharedKernel.Errors.Models;

public static class AuthErrors
{
    public const string PrefixCode = "error/auth";
    public const string PrefixMessageKey = "Error.Auth";

    public static readonly ErrorDefinition Unauthorized = new(
        $"{PrefixCode}/unauthorized",
        $"{PrefixMessageKey}.Unauthorized"
    );

    public static readonly ErrorDefinition Forbidden = new(
        $"{PrefixCode}/forbidden",
        $"{PrefixMessageKey}.Forbidden"
    );
}