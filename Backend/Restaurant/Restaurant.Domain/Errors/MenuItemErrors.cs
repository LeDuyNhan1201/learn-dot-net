using BuildingBlocks.Shared.Errors.Models;

namespace Restaurant.Domain.Errors;

public static class MenuItemErrors
{
    private const string PrefixCode = "error/menu-item";
    private const string PrefixMessageKey = "Error.MenuItem";

    public static readonly ErrorDefinition InvalidPrice = new(
        $"{PrefixCode}/invalid-price",
        $"{PrefixMessageKey}.InvalidPrice"
    );
}