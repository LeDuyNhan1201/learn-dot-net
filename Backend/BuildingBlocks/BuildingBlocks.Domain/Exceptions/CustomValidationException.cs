using BuildingBlocks.SharedKernel.Errors.Models;

namespace BuildingBlocks.Domain.Exceptions;

public sealed class CustomValidationException : Exception
{
    public CustomValidationException(
        (
            IReadOnlyDictionary<string, ValidationError[]> Errors,
            IReadOnlyList<string> OtherErrors
            ) validation)
        : this(validation.Errors, validation.OtherErrors)
    {
    }

    public CustomValidationException(
        IReadOnlyDictionary<string, ValidationError[]> errors,
        IReadOnlyList<string>? otherErrors = null)
        : base(otherErrors is { Count: > 0 }
            ? string.Join(Environment.NewLine, otherErrors)
            : "One or more validation failures have occurred.")
    {
        Errors = errors;
        OtherErrors = otherErrors ?? [];
    }

    public IReadOnlyDictionary<string, ValidationError[]> Errors { get; }

    public IReadOnlyList<string> OtherErrors { get; }
}