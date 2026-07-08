using BuildingBlocks.API.Extensions;
using BuildingBlocks.Domain.Exceptions.Handlers;
using BuildingBlocks.Domain.Validation;
using BuildingBlocks.Infrastructure.Authentication.Extensions;
using BuildingBlocks.Infrastructure.Authentication.keycloakAdmin.Extensions;
using BuildingBlocks.Infrastructure.Observability.Extensions;
using BuildingBlocks.Infrastructure.OpenApi.Extensions;
using BuildingBlocks.Infrastructure.Persistence.Extensions;
using FluentValidation;
using Microsoft.IdentityModel.Logging;
using Restaurant.Application.Contracts;
using Restaurant.Application.Services;
using Restaurant.Application.Services.Interfaces;
using Restaurant.Application.Validation.Validators;
using Restaurant.Infrastructure.Persistence;

namespace Restaurant.API.Extensions;

/// <summary>
///     Extension methods for configuring services in the WebApplicationBuilder.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Configures services for the WebApplicationBuilder.
    /// </summary>
    /// <param name="builder">The WebApplicationBuilder instance to configure services for.</param>
    /// <returns>The configured WebApplicationBuilder instance.</returns>
    public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;
        var environment = builder.Environment;

        if (environment.IsDevelopment() || environment.IsEnvironment("Local")) IdentityModelEventSource.ShowPII = true;

        services
            .AddBaseOptions()
            .AddPostgresDatabase<RestaurantDbContext>()
            .AddI18NLocalization()
            .AddScalarOpenApi();

        if (!environment.IsEnvironment("Local")) services.AddObservability(configuration);
        // services.AddScoped<IMenuItemValidator.CreateRequest>();
        // services.AddScoped<IValidator<IMenuItemDto.CreateRequest>, IMenuItemValidator.CreateRequest>();
        services.AddValidatorsFromAssemblyContaining<IMenuItemValidator>();

        // services.AddScoped(typeof(ValidationFilter<,>));
        // services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblyContaining<IMenuItemCommand>();
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddKeycloakAdmin(configuration);

        services.AddAuthenticationWithAuthorization(configuration, environment);

        services.AddScoped<ITodoService, TodoService>();

        services.ConfigureApplicationJson();

        services.AddExceptionHandler<HttpExceptionHandler<Messages>>();
        services.AddProblemDetails();

        return builder;
    }
}