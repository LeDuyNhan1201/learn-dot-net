using API.Configurations;
using API.Options;
using API.Serialization;
using API.Serialization.Converter;
using Application.Interfaces;
using Application.Services;

namespace API.Extensions;

public static class DependencyInjection
{
    public static WebApplicationBuilder AddApplicationServices(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);

            options.SerializerOptions.Converters.Add(new DateOnlyJsonConverter());
        });

        builder.Services.AddAppLocalization();

        builder.Services.AddOptionsConfiguration(builder.Configuration);

        builder.Services.AddApiServices();

        builder.Services.AddOpenApi();

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
        services.Configure<AppOptions>(configuration.GetSection("App"));
        
        // TODO: Inject more options here
        
        return services;
    }
}