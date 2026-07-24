using BuildingBlocks.Domain.ExecutionContext.Interfaces;
using BuildingBlocks.Domain.Repositories.UnitOfWork;
using BuildingBlocks.Persistence.ExecutionContext;
using BuildingBlocks.Persistence.Interceptors;
using BuildingBlocks.Persistence.Options;
using BuildingBlocks.Persistence.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BuildingBlocks.Persistence.Extensions;

public static class DatabaseExtensions
{
    public static IServiceCollection AddPostgresDatabase<T>(this IServiceCollection services) where T : DbContext
    {
        services.AddScoped<AuditInterceptor>();

        services.AddDbContext<T>((provider, options) =>
        {
            var environment = provider.GetRequiredService<IHostEnvironment>();

            if (!environment.IsProduction())
                options
                    .EnableSensitiveDataLogging()
                    .EnableDetailedErrors();

            var postgresOptions = provider.GetRequiredService<IOptions<PostgresOptions>>().Value;
            options
                .UseLazyLoadingProxies()
                .UseNpgsql(postgresOptions.GetConnectionString())
                .AddInterceptors(provider.GetRequiredService<AuditInterceptor>());
        });

        services.AddSingleton<IExecutionContextAccessor, ExecutionContextAccessor>();

        services.AddScoped<IExecutionContextInitializer, ExecutionContextInitializer>();
        
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }

    public static async Task<WebApplication> AutoMigration<T>(this WebApplication app) where T : DbContext
    {
        await using var scope = app.Services.CreateAsyncScope();

        var logger = scope.ServiceProvider.GetRequiredService<ILogger<T>>();
        var dbContext = scope.ServiceProvider.GetRequiredService<T>();

        logger.LogInformation("Applying database migrations...");

        await dbContext.Database.MigrateAsync();

        logger.LogInformation("Database migrations completed.");

        return app;
    }
}