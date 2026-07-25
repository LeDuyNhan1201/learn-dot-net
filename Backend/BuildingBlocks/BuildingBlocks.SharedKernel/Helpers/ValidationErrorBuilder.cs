using System.Text.Json;
using BuildingBlocks.SharedKernel.Errors.Models;
using FluentValidation.Results;

namespace BuildingBlocks.SharedKernel.Helpers;

public static class ValidationErrorBuilder
{
    private static readonly string[] Prefixes =
    [
        "ICreate",
        "IUpdate",
        "IDelete",
        "I",

        "Create",
        "Update",
        "Delete"
    ];

    private static readonly string[] Suffixes =
    [
        "Dto+CreateRequest",
        "Dto+UpdateRequest",
        "Dto+DeleteRequest",
        "Command+Create",
        "Command+Update",
        "Command+Delete",

        "Request",
        "Command"
    ];

    public static (
        IReadOnlyDictionary<string, ValidationError[]> Errors,
        IReadOnlyList<string> OtherErrors) Build<T>(
            T model,
            IEnumerable<ValidationFailure> failures)
    {
        var validationErrors = failures
            .Select(failure => new
            {
                Field = NormalizeFieldName(model, failure.PropertyName),
                Failure = failure
            })
            .ToList();

        var errors = validationErrors
            .Where(x => !string.IsNullOrWhiteSpace(x.Field))
            .GroupBy(x => x.Field)
            .ToDictionary(
                g => g.Key,
                g => g.Select(x => ToValidationError(x.Failure)).ToArray());

        var otherErrors = validationErrors
            .Where(x => string.IsNullOrWhiteSpace(x.Field))
            .Select(x => x.Failure.ErrorMessage)
            .ToArray();

        return (errors, otherErrors);
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