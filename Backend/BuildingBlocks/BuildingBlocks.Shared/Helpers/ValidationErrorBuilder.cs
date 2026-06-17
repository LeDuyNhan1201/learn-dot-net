using System.Text.Json;
using BuildingBlocks.Shared.Errors.Models;
using FluentValidation.Results;

namespace BuildingBlocks.Shared.Helpers;

public static class ValidationErrorBuilder
{
    private static readonly string[] Prefixes =
    [
        "ICreate",
        "IUpdate",
        "IDelete",
        "I"
    ];

    private static readonly string[] Suffixes =
    [
        "Dto+CreateRequest",
        "Dto+UpdateRequest",
        "Dto+DeleteRequest",
        "Command+Create",
        "Command+Update",
        "Command+Delete"
    ];

    public static Dictionary<string, ValidationError[]> Build<T>(
        T model,
        IEnumerable<ValidationFailure> failures)
    {
        return failures
            .GroupBy(failure => NormalizeFieldName(model, failure.PropertyName))
            .ToDictionary(
                grouping => grouping.Key,
                grouping => grouping.Select(ToValidationError).ToArray());
    }

    private static ValidationError ToValidationError(ValidationFailure failure)
    {
        var arguments = failure.FormattedMessagePlaceholderValues
            .Where(pair => pair.Key is not ("PropertyName" or "PropertyValue"))
            .Select(pair => pair.Value!)
            .ToArray();

        return new ValidationError(
            failure.PropertyName,
            failure.ErrorMessage,
            arguments);
    }

    private static string NormalizeFieldName<T>(
        T model,
        string fieldName)
    {
        var modelName = model!.GetType().FullName!.Split('.').Last();

        foreach (var prefix in Prefixes)
            if (modelName.StartsWith(prefix))
            {
                modelName = modelName[prefix.Length..];
                break;
            }

        foreach (var suffix in Suffixes)
            if (modelName.EndsWith(suffix))
            {
                modelName = modelName[..^suffix.Length];
                break;
            }

        return JsonNamingPolicy.CamelCase.ConvertName(
            fieldName.Replace(
                modelName,
                string.Empty,
                StringComparison.Ordinal));
    }
}