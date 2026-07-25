using BuildingBlocks.Domain.Exceptions.Processors;
using BuildingBlocks.SharedKernel.DTOs;
using BuildingBlocks.SharedKernel.Errors.Models;
using BuildingBlocks.SharedKernel.Localization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Domain.Exceptions.Handlers;

public sealed class HttpExceptionHandler<TMessage>(
    IExceptionProcessor processor,
    IExceptionMapper mapper,
    CompositeLocalizer<TMessage> localizer)
    : IExceptionHandler
    where TMessage : class
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext context,
        Exception exception,
        CancellationToken cancellationToken)
    {
        await processor.ProcessAsync(
            exception,
            cancellationToken);

        var result = mapper.Map(exception);

        context.Response.StatusCode = result.StatusCode;

        if (result.Errors is not null || result.OtherErrors is not null)
        {
            await context.Response.WriteAsJsonAsync(
                new CustomValidationResponse
                {
                    Code = result.Code,
                    Message = localizer[result.MessageKey],
                    Errors = result.Errors?.Count > 0
                        ? result.Errors.ToDictionary(
                            x => x.Key,
                            x => x.Value.Select(Localize).ToArray())
                        : null,
                    OtherErrors = result.OtherErrors?.Count > 0
                        ? result.OtherErrors
                            .Select(x => localizer[x])
                            .ToArray()
                        : null
                },
                cancellationToken);

            return true;
        }

        await context.Response.WriteAsJsonAsync(
            new BaseResponse<Dictionary<string, string>?>
            {
                Code = result.Code,
                Message = localizer[result.MessageKey],
                Data = result.Data?.Count > 0
                    ? result.Data
                    : null
            },
            cancellationToken);

        return true;
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