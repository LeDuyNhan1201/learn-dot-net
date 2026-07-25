using BuildingBlocks.SharedKernel.Errors.Models;
using FluentValidation;
using FluentValidation.Validators;

namespace Restaurant.Application.Validation.Rules;

public sealed class MenuItemPriceValidator<T> : PropertyValidator<T, decimal>
{
    public override string Name => nameof(MenuItemPriceValidator<T>);

    public override bool IsValid(ValidationContext<T> context, decimal value)
    {
        return value > 0;
    }

    protected override string GetDefaultMessageTemplate(string errorCode)
    {
        return ValidationErrors.Valid.MessageKey;
    }
}