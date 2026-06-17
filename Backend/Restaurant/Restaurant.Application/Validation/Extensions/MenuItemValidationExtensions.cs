using BuildingBlocks.Application.Validation.Extensions;
using BuildingBlocks.Shared.Errors.Models;
using FluentValidation;
using Restaurant.Application.Validation.Rules;

namespace Restaurant.Application.Validation.Extensions;

public static class MenuItemValidationExtensions
{
    public static IRuleBuilderOptions<T, decimal> MenuItemPrice<T>(this IRuleBuilder<T, decimal> rule)
    {
        return rule
            .SetValidator(new MenuItemPriceValidator<T>())
            .WithError(ValidationErrors.Valid);
    }
}