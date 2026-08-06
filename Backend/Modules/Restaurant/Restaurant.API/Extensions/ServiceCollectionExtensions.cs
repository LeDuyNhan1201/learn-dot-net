using BuildingBlocks.API.Extensions;
using BuildingBlocks.Application.Behaviors;
using BuildingBlocks.Domain.Contracts;
using BuildingBlocks.Domain.Exceptions.Handlers;
using BuildingBlocks.Identity.Extensions;
using BuildingBlocks.Identity.keycloakAdmin.Extensions;
using BuildingBlocks.Messaging.Extensions;
using BuildingBlocks.Observability.Extensions;
using BuildingBlocks.OpenApi.Extensions;
using BuildingBlocks.Persistence.Extensions;
using BuildingBlocks.SharedKernel.Localization;
using FluentValidation;
using Microsoft.IdentityModel.Logging;
using Restaurant.Application.Consumers;
using Restaurant.Application.Handlers.Command;
using Restaurant.Application.Handlers.DomainEvent;
using Restaurant.Application.Services;
using Restaurant.Application.Services.Interfaces;
using Restaurant.Application.Validation.Validators;
using Restaurant.Domain.Contracts.Commands;
using Restaurant.Domain.Contracts.DomainEvents;
using Restaurant.Domain.Repositories;
using Restaurant.Infrastructure.OpenApi;
using Restaurant.Infrastructure.Persistence;
using Restaurant.Infrastructure.Persistence.Repositories;

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
            .AddMessaging<RestaurantDbContext>(typeof(MenuItemCreatedConsumer))
            .AddPostgresDatabase<RestaurantDbContext>()
            .AddI18NLocalization()
            .AddExceptionObservability()
            .AddScalarOpenApi(options => { options.AddSchemaTransformer<SampleSchemaOperationTransformer>(); });

        services.AddObservability(configuration);

        services.AddScoped<IDomainEventHandler<MenuItemCreatedDomainEvent>, CreateMenuItemDomainEventHandler>();

        services.AddValidatorsFromAssemblyContaining<CreateMenuItemCommandValidator>();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblyContaining<CreateMenuItemCommand>();
            cfg.RegisterServicesFromAssemblyContaining<CreateMenuItemCommandHandler>();
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
            cfg.AddOpenBehavior(typeof(PerformanceBehavior<,>));
        });

        services.AddScoped<IMenuItemRepository, MenuItemRepository>();
        
        services.AddKeycloakAdmin(configuration);

        services.AddAuthenticationWithAuthorization(configuration, environment);

        services.AddScoped<ITodoService, TodoService>();

        services.ConfigureApplicationJson();

        services.AddExceptionHandler<HttpExceptionHandler<Messages>>();
        services.AddProblemDetails();

        return builder;
    }
}