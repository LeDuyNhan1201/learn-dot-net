using BuildingBlocks.SharedKernel.Errors.Models;

namespace BuildingBlocks.Domain.Exceptions;

public sealed class AppException(int httpCode, ErrorDefinition definition) : Exception(definition.MessageKey)
{
    public int HttpCode { get; } = httpCode;
    public ErrorDefinition ErrorDefinition { get; } = definition;
}