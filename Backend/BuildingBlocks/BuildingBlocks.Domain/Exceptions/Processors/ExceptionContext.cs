using BuildingBlocks.SharedKernel.Errors.Models;

namespace BuildingBlocks.Domain.Exceptions.Processors;

public sealed record ExceptionContext(
    int StatusCode,
    string Code,
    string MessageKey,
    IReadOnlyDictionary<string, ValidationError[]>? Errors = null,
    IReadOnlyList<string>? OtherErrors = null,
    Dictionary<string, string>? Data = null);