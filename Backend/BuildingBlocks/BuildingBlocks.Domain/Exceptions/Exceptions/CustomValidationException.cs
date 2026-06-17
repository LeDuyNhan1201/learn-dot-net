using BuildingBlocks.Shared.Errors.Models;

namespace BuildingBlocks.Domain.Exceptions.Exceptions;

public sealed class CustomValidationException(IDictionary<string, ValidationError[]> errors) : Exception
{
    public IDictionary<string, ValidationError[]> Errors { get; } = errors;
}