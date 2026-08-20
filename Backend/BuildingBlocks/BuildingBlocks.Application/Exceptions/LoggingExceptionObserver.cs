using BuildingBlocks.Domain.Abstractions.Exception;
using BuildingBlocks.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Application.Exceptions;

public sealed class LoggingExceptionObserver(ILogger<LoggingExceptionObserver> logger) : IExceptionObserver
{
    public Task ObserveAsync(Exception exception, CancellationToken cancellationToken = default)
    {
        if (exception is not AppException &&
            exception is not CustomValidationException &&
            exception is not OperationCanceledException)
            logger.LogError(exception, "Unhandled exception");

        return Task.CompletedTask;
    }
}