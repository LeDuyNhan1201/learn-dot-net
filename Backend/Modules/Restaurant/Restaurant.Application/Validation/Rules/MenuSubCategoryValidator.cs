using BuildingBlocks.SharedKernel.Errors.Models;
using FluentValidation;
using FluentValidation.Validators;
using Restaurant.Domain.Enumerations;

namespace Restaurant.Application.Validation.Rules;

public sealed class MenuSubCategoryValidator<T> : PropertyValidator<T, MenuSubCategory>
{
    public override string Name => nameof(MenuSubCategoryValidator<T>);

    public override bool IsValid(ValidationContext<T> context, MenuSubCategory value)
    {
        return Enum.IsDefined(value);
    }

    protected override string GetDefaultMessageTemplate(string errorCode)
    {
        return ValidationErrors.Valid.MessageKey;
    }
}