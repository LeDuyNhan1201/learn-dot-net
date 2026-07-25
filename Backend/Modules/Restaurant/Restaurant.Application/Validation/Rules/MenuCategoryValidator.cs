using BuildingBlocks.SharedKernel.Errors.Models;
using FluentValidation;
using FluentValidation.Validators;
using Restaurant.Domain.Enumerations;

namespace Restaurant.Application.Validation.Rules;

public sealed class MenuCategoryValidator<T> : PropertyValidator<T, MenuCategory>
{
    public override string Name => nameof(MenuCategoryValidator<T>);

    public override bool IsValid(ValidationContext<T> context, MenuCategory value)
    {
        return Enum.IsDefined(value);
    }

    protected override string GetDefaultMessageTemplate(string errorCode)
    {
        return ValidationErrors.Valid.MessageKey;
    }
}