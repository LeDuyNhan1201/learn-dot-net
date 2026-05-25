using BuildingBlocks.API.Serialization;
using BuildingBlocks.API.Serialization.Converter;
using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Application.Options;
using BuildingBlocks.Infrastructure.Observability;
using BuildingBlocks.Infrastructure.OpenApi;
using BuildingBlocks.Infrastructure.OpenApi.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.API.Extensions;

public static class DependencyExtensions
{
    public static IServiceCollection AddSerializations(
        this IServiceCollection services,
        Action<SerializationBuilder>? configure = null)
    {
        var builder = new SerializationBuilder();

        configure?.Invoke(builder);

        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.TypeInfoResolverChain.Insert(0, CommonJsonSerializerContext.Default);

            foreach (var resolver in builder.Resolvers)
            {
                options.SerializerOptions.TypeInfoResolverChain.Insert(0, resolver);
            }

            options.SerializerOptions.Converters.Add(new DateOnlyJsonConverter());

            foreach (var converter in builder.Converters)
            {
                options.SerializerOptions.Converters.Add(converter);
            }
        });

        return services;
    }
    
    public static IServiceCollection AddApiServices(
        this IServiceCollection services,
        params IServiceModule[] modules)
    {
        foreach (var module in modules)
        {
            module.Register(services);
        }

        return services;
    }

    public static IServiceCollection AddOptionsConfiguration(
        this IServiceCollection services,
        IConfiguration configuration,
        params IOptionsModule[] modules)
    {
        services
            .AddOptions<ServerOptions>()
            .BindConfiguration(ServerOptions.SectionName)
            .ValidateOnStart();

        services
            .AddOptions<ObservabilityOptions>()
            .BindConfiguration(ObservabilityOptions.SectionName)
            .ValidateOnStart();

        services
            .AddOptions<ApiDocsOptions>()
            .BindConfiguration(ApiDocsOptions.SectionName)
            .ValidateOnStart();

        foreach (var module in modules)
        {
            module.Register(services, configuration);
        }

        return services;
    }
    
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        params IInfrastructureModule[] modules)
    {
        services
            .AddObservability(configuration)
            .AddScalarOpenApi();

        foreach (var module in modules)
        {
            module.Register(services, configuration);
        }

        return services;
    }
    
}