using BuildingBlocks.SharedKernel.Errors.Models;
using FluentValidation;

namespace BuildingBlocks.Application.Extensions;

public static class ValidationExtensions
{
    public static IRuleBuilderOptions<T, TProperty> WithError<T, TProperty>(
        this IRuleBuilderOptions<T, TProperty> rule, ErrorDefinition error)
    {
        return rule
            .WithErrorCode(error.Code)
            .WithMessage(error.MessageKey);
    }

    public static IRuleBuilderOptions<T, TProperty> Required<T, TProperty>(this IRuleBuilder<T, TProperty> rule)
    {
        return rule
            .NotEmpty()
            .WithError(ValidationErrors.Required)
            .NotNull()
            .WithError(ValidationErrors.Required);
    }

    public static IRuleBuilderOptions<T, string> Email<T>(this IRuleBuilder<T, string> rule)
    {
        return rule
            .EmailAddress()
            .WithError(ValidationErrors.Email);
    }

    public static IRuleBuilderOptions<T, string> MaxLength<T>(this IRuleBuilder<T, string> rule, int length)
    {
        return rule
            .MaximumLength(length)
            .WithError(ValidationErrors.Max);
    }


    public static IRuleBuilderOptions<T, string> MinLength<T>(this IRuleBuilder<T, string> rule, int length)
    {
        return rule
            .MinimumLength(length)
            .WithError(ValidationErrors.Min);
    }

    public static IRuleBuilderOptions<T, int> Range<T>(this IRuleBuilder<T, int> rule, int min, int max)
    {
        return rule
            .ExclusiveBetween(min, max)
            .WithError(ValidationErrors.Range);
    }

    public static IRuleBuilderOptions<T, long> Range<T>(this IRuleBuilder<T, long> rule, int min, int max)
    {
        return rule
            .ExclusiveBetween(min, max)
            .WithError(ValidationErrors.Range);
    }

    public static IRuleBuilderOptions<T, decimal> Range<T>(this IRuleBuilder<T, decimal> rule, int min, int max)
    {
        return rule
            .ExclusiveBetween(min, max)
            .WithError(ValidationErrors.Range);
    }
}