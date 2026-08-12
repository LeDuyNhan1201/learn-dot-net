using BuildingBlocks.API.Extensions;
using BuildingBlocks.Application.Behaviors;
using BuildingBlocks.Application.Services;
using BuildingBlocks.Domain.Contracts;
using BuildingBlocks.Domain.Exceptions.Handlers;
using BuildingBlocks.Domain.Services;
using BuildingBlocks.Identity.Extensions;
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

        // Inject Infrastructure
        services
            .AddBaseOptions()
            .AddMessaging<RestaurantDbContext>(typeof(MenuItemCreatedConsumer))
            .AddPostgresDatabase<RestaurantDbContext>()
            .AddI18NLocalization()
            .AddExceptionObservability()
            .AddObservability(configuration)
            .AddScalarOpenApi(options => { options.AddSchemaTransformer<SampleSchemaOperationTransformer>(); });

        services.AddValidatorsFromAssemblyContaining<CreateMenuItemCommandValidator>();
        
        // Inject Domain Event Handlers
        services.AddScoped<IDomainEventHandler<MenuItemCreatedDomainEvent>, CreateMenuItemDomainEventHandler>();

        // Inject MediatR Components
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblyContaining<CreateMenuItemCommand>();
            cfg.RegisterServicesFromAssemblyContaining<CreateMenuItemCommandHandler>();
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
            cfg.AddOpenBehavior(typeof(PerformanceBehavior<,>));
        });

        // Inject Repositories
        services.AddScoped<IMenuItemRepository, MenuItemRepository>();
        
        // Inject Security
        services.AddKeycloakAdmin(configuration, environment);
        services.AddAuthenticationWithAuthorization(configuration, environment);

        // Inject Services
        services.AddScoped<ITodoService, TodoService>();
        services.AddScoped<IUserSeederService, UserSeederService>();

        // Other Configurations
        services.ConfigureApplicationJson();
        services.AddExceptionHandler<HttpExceptionHandler<Messages>>();
        services.AddProblemDetails();

        return builder;
    }
}