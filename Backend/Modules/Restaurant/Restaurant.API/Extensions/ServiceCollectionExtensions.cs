using BuildingBlocks.API.Extensions;
using BuildingBlocks.Application.Behaviors;
using BuildingBlocks.Domain.Exceptions.Handlers;
using BuildingBlocks.Identity.Extensions;
using BuildingBlocks.Identity.keycloakAdmin.Extensions;
using BuildingBlocks.Observability.Extensions;
using BuildingBlocks.OpenApi.Extensions;
using BuildingBlocks.Persistence.Extensions;
using BuildingBlocks.SharedKernel.Localization;
using FluentValidation;
using Microsoft.IdentityModel.Logging;
using Restaurant.Application.Services;
using Restaurant.Application.Services.Interfaces;
using Restaurant.Application.Validation.Validators;
using Restaurant.Domain.Contracts;
using Restaurant.Infrastructure.Persistence;

namespace Restaurant.API.Extensions;

public static class ServiceCollectionExtensions
{
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
            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>)); 
            cfg.AddOpenBehavior(typeof(PerformanceBehavior<,>));
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