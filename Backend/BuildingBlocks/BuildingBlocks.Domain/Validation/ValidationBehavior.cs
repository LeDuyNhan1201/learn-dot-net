using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.SharedKernel.Helpers;
using FluentValidation;
using MediatR;

namespace BuildingBlocks.Domain.Validation;

public sealed class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!validators.Any()) return await next(cancellationToken);

        var context = new ValidationContext<TRequest>(request);

        var failures = (
                await Task.WhenAll(
                    validators.Select(validator => validator.ValidateAsync(context, cancellationToken))
                )
            )
            .SelectMany(result => result.Errors)
            .Where(failure => failure is not null)
            .ToList();

        if (failures.Count != 0) throw new CustomValidationException(ValidationErrorBuilder.Build(request, failures));

        return await next(cancellationToken);
    }
}