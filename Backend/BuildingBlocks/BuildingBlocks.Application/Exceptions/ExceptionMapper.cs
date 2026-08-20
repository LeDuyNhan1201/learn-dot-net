using System.Collections;
using BuildingBlocks.Domain.Abstractions.Exception;
using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.SharedKernel.Errors.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace BuildingBlocks.Application.Exceptions;

public sealed class ExceptionMapper(IHostEnvironment environment) : IExceptionMapper
{
    public ExceptionContext Map(Exception exception)
    {
        return exception switch
        {
            AppException appException
                => new ExceptionContext(
                    appException.HttpCode,
                    appException.ErrorDefinition.Code,
                    appException.ErrorDefinition.MessageKey,
                    Data: BuildExceptionData(
                        exception,
                        !environment.IsProduction())),

            CustomValidationException validation
                => new ExceptionContext(
                    StatusCodes.Status400BadRequest,
                    ValidationErrors.PrefixCode,
                    ValidationErrors.PrefixMessageKey,
                    validation.Errors,
                    validation.OtherErrors),

            _ => new ExceptionContext(
                StatusCodes.Status500InternalServerError,
                "error/internal-server-error",
                "Error.InternalServer",
                Data: BuildExceptionData(
                    exception,
                    !environment.IsProduction()))
        };
    }

    private static Dictionary<string, string> BuildExceptionData(Exception exception, bool includeDetails)
    {
        var data = exception.Data
            .Cast<DictionaryEntry>()
            .ToDictionary(
                entry => entry.Key.ToString()!,
                entry => entry.Value?.ToString() ?? string.Empty);

        if (!includeDetails)
            return data;

        data["ExceptionMessage"] = exception.Message;
        data["StackTrace"] = exception.StackTrace ?? "";
        data["InnerException"] = exception.InnerException?.ToString() ?? "";

        return data;
    }
}