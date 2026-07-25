using BuildingBlocks.Application.Extensions;
using BuildingBlocks.SharedKernel.Errors.Models;
using FluentValidation;
using Restaurant.Application.Validation.Rules;
using Restaurant.Domain.Contracts;
using Restaurant.Domain.Enumerations;
using Restaurant.Domain.Errors;

namespace Restaurant.Application.Validation.Extensions;

public static class MenuItemValidationExtensions
{
    public static IRuleBuilderOptions<T, decimal> MenuItemPrice<T>(this IRuleBuilder<T, decimal> rule)
    {
        return rule
            .SetValidator(new MenuItemPriceValidator<T>())
            .WithError(ValidationErrors.Valid);
    }

    public static IRuleBuilderOptions<T, MenuSubCategory> MenuItemPrice<T>(this IRuleBuilder<T, MenuSubCategory> rule)
    {
        return rule
            .SetValidator(new MenuSubCategoryValidator<T>())
            .WithError(ValidationErrors.Valid);
    }

    public static IRuleBuilderOptions<T, MenuCategory> MenuItemPrice<T>(this IRuleBuilder<T, MenuCategory> rule)
    {
        return rule
            .SetValidator(new MenuCategoryValidator<T>())
            .WithError(ValidationErrors.Valid);
    }

    public static IRuleBuilderOptions<T, T> ValidCategoryMapping<T>(this IRuleBuilder<T, T> ruleBuilder)
        where T : CreateMenuItemCommand
    {
        return ruleBuilder
            .Must(x =>
                x.Category switch
                {
                    MenuCategory.Food => x.SubCategory is
                        MenuSubCategory.Breakfast or
                        MenuSubCategory.Lunch or
                        MenuSubCategory.Dinner,

                    MenuCategory.Drink => x.SubCategory is
                        MenuSubCategory.SoftDrink or
                        MenuSubCategory.Alcohol,

                    _ => false
                })
            .WithMessage(MenuItemErrors.InvalidCategoryMapping.MessageKey);
    }
}