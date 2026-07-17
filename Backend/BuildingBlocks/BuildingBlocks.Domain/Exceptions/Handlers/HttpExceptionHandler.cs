using System.Collections;
using BuildingBlocks.SharedKernel.DTOs;
using BuildingBlocks.SharedKernel.Errors.Models;
using BuildingBlocks.SharedKernel.Localization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Domain.Exceptions.Handlers;

public sealed class HttpExceptionHandler<TMessage>(
    ILogger<HttpExceptionHandler<TMessage>> logger,
    IHostEnvironment environment,
    CompositeLocalizer<TMessage> localizer)
    : IExceptionHandler
    where TMessage : class
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext context,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is CustomValidationException validation)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;

            await context.Response.WriteAsJsonAsync(
                new BaseResponse<IDictionary<string, string[]>>
                {
                    Code = ValidationErrors.PrefixCode,
                    Message = localizer[ValidationErrors.PrefixMessageKey],
                    Data = validation.Errors.ToDictionary(
                        field => field.Key,
                        field => field.Value.Select(Localize).ToArray())
                },
                cancellationToken);

            return true;
        }

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        var data = BuildExceptionData(
            exception,
            !environment.IsProduction());

        logger.LogError(exception, "Unhandled exception occurred");

        await context.Response.WriteAsJsonAsync(
            new BaseResponse<Dictionary<string, string>>
            {
                Code = "error/internal-server-error",
                Message = localizer["Error.InternalServer"],
                Data = data
            },
            cancellationToken);

        return true;
    }

    private static Dictionary<string, string> BuildExceptionData(
        Exception exception,
        bool includeDetails)
    {
        var data = exception.Data
            .Cast<DictionaryEntry>()
            .ToDictionary(
                entry => entry.Key.ToString()!,
                entry => entry.Value?.ToString() ?? string.Empty);

        if (!includeDetails) return data;

        data["ExceptionMessage"] = exception.Message;
        data["StackTrace"] = exception.StackTrace ?? string.Empty;
        data["InnerException"] = exception.InnerException?.ToString() ?? string.Empty;

        return data;
    }

    private string Localize(ValidationError error)
    {
        var args = error.Arguments
            .Prepend(localizer[$"Field.{error.Field}"])
            .Distinct()
            .ToArray();

        return localizer[error.MessageKey, args];
    }
}