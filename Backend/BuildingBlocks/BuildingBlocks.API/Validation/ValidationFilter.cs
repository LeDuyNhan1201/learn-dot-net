using BuildingBlocks.Shared.DTOs;
using BuildingBlocks.Shared.Errors.Models;
using BuildingBlocks.Shared.Helpers;
using BuildingBlocks.Shared.Localization;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.API.Validation;

public sealed class ValidationFilter<T, TMessage>(
    CompositeLocalizer<TMessage> localizer)
    : IEndpointFilter
    where TMessage : class
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var validator = context.HttpContext
            .RequestServices
            .GetService<IValidator<T>>();

        var model = context.Arguments
            .OfType<T>()
            .FirstOrDefault();

        if (validator is null || model is null) return await next(context);

        var result = await validator.ValidateAsync(
            model,
            context.HttpContext.RequestAborted);

        if (result.IsValid) return await next(context);

        var validationError = ValidationErrorBuilder.Build(model, result.Errors);
        return Results.BadRequest(
            new BaseResponse<Dictionary<string, string[]>>
            {
                Code = ValidationErrors.PrefixCode,
                Message = localizer[ValidationErrors.PrefixMessageKey],
                Data = validationError.ToDictionary(
                    field => field.Key,
                    field => field.Value.Select(Localize).ToArray())
            });
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