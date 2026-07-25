using BuildingBlocks.SharedKernel.Errors.Models;

namespace Restaurant.Domain.Errors;

public static class MenuItemErrors
{
    private const string PrefixCode = "error/menu-item";
    private const string PrefixMessageKey = "Error.MenuItem";

    public static readonly ErrorDefinition InvalidPrice = new(
        $"{PrefixCode}/invalid-price",
        $"{PrefixMessageKey}.InvalidPrice"
    );
    
    public static readonly ErrorDefinition InvalidCategory = new(
        $"{PrefixCode}/invalid-category",
        $"{PrefixMessageKey}.InvalidCategory"
    );
    
    public static readonly ErrorDefinition InvalidSubCategory = new(
        $"{PrefixCode}/invalid-sub-category",
        $"{PrefixMessageKey}.InvalidSubCategory"
    );
    
    public static readonly ErrorDefinition InvalidCategoryMapping = new(
        $"{PrefixCode}/invalid-category-mapping",
        $"{PrefixMessageKey}.InvalidCategoryMapping"
    );
}