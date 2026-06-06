using System.Text.Json;
using BuildingBlocks.Shared.DTOs;
using BuildingBlocks.Shared.Errors.Models;
using BuildingBlocks.Shared.Localization;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.API.Validation;

public abstract class ValidatorBase
{
    protected static readonly string[] Prefixes =
    [
        "Create",
        "Update",
        "Delete"
    ];

    protected static readonly string[] Suffixes =
    [
        "Dto",
        "RequestDto",
        "ResponseDto"
    ];
}

public sealed class ValidationFilter<T, TMessage>(
    CompositeLocalizer<TMessage> localizer)
    : ValidatorBase, IEndpointFilter
    where TMessage : class
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var validator = context.HttpContext
            .RequestServices
            .GetService<IValidator<T>>();

        if (validator is null) return await next(context);
        var model = context.Arguments.OfType<T>().FirstOrDefault();
        if (model is null) return await next(context);

        var result = await validator.ValidateAsync(model);
        if (result.IsValid) return await next(context);

        var errors = result.Errors
            .GroupBy(failure => failure.PropertyName)
            .ToDictionary(
                failures => NormalizeFieldName(model, failures.Key),
                failures => failures.Select(Localize).ToArray());

        return Results.BadRequest(
            new BaseResponse<IDictionary<string, string[]>>(
                ValidationErrors.PrefixCode,
                localizer[ValidationErrors.PrefixMessageKey],
                errors));
    }

    private string Localize(ValidationFailure failure)
    {
        return localizer[failure.ErrorMessage, BuildArguments(failure)];
    }

    private object[] BuildArguments(ValidationFailure failure)
    {
        return failure.FormattedMessagePlaceholderValues
            .Where(pair => pair.Key is not ("PropertyName" or "PropertyValue"))
            .Select(pair => pair.Value!)
            .Prepend(localizer[$"Field.{failure.PropertyName}"])
            .Distinct().ToArray();
    }

    private static string NormalizeFieldName(T? model, string fieldName)
    {
        var modelName = model!.GetType().Name;

        modelName = Prefixes
            .FirstOrDefault(p => modelName.StartsWith(p, StringComparison.Ordinal)) is { } prefix
            ? modelName[prefix.Length..]
            : modelName;

        modelName = Suffixes
            .FirstOrDefault(s => modelName.EndsWith(s, StringComparison.Ordinal)) is { } suffix
            ? modelName[..^suffix.Length]
            : modelName;

        return JsonNamingPolicy.CamelCase.ConvertName(fieldName.Replace(modelName, string.Empty, StringComparison.Ordinal));
    }
}