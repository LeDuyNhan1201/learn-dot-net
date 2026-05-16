using API.Serialization;
using API.Serialization.Converter;
using Application.Interfaces;
using Application.Services;
using Infrastructure.Observability;
using Infrastructure.Options;
using Infrastructure.Swagger;

namespace API.Extensions;

public static class DependencyInjection
{
    public static WebApplicationBuilder AddApplicationServices(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Services
            .ConfigureHttpJsonOptions(options =>
            {
                options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
                options.SerializerOptions.Converters.Add(new DateOnlyJsonConverter());
            })

            .AddAppLocalization()

            .AddOptionsConfiguration(builder.Configuration)

            .AddInfrastructure(builder.Configuration)

            .AddApiServices()

            .AddOpenApi();

        return builder;
    }
    
    private static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddScoped<ITodoService, TodoService>();
        
        // TODO: Inject more services here
        
        return services;
    }
    
    private static IServiceCollection AddOptionsConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ServerOptions>(configuration.GetSection(ServerOptions.SectionName));
        
        services.Configure<ObservabilityOptions>(configuration.GetSection(ObservabilityOptions.SectionName));
        
        // TODO: Inject more options here
        
        return services;
    }
    
    private static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddObservability(configuration);
        
        services.AddCustomSwagger(configuration);

        // TODO: Inject more infrastructure services here
        
        return services;
    }
}