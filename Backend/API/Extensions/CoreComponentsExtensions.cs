using API.Serialization;
using API.Serialization.Converter;
using Application.Interfaces;
using Application.Services;
using Infrastructure.Observability;
using Infrastructure.OpenApi;
using Infrastructure.Options;

namespace API.Extensions;

public static class CoreComponentsExtensions
{
    public static WebApplicationBuilder AddCoreComponents(this WebApplicationBuilder builder)
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
            .AddApiServices();

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

        services.Configure<OpenApiOptions>(configuration.GetSection(OpenApiOptions.SectionName));
        
        // TODO: Inject more options here

        return services;
    }

    private static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddObservability(configuration)
            
            .AddOpenAPI(configuration);

        // TODO: Inject more infrastructure services here

        return services;
    }
}