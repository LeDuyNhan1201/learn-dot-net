using API.Serialization;
using API.Serialization.Converter;
using Application.Interfaces;
using Application.Services;
using Infrastructure.Observability;
using Infrastructure.Options;
using Infrastructure.Swagger;
using Microsoft.OpenApi;

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
            
            .AddOpenApi(builder.Configuration["Swagger:ApiDocs"] ?? "learn-dot-net-api-docs", options =>
            {
                options.AddDocumentTransformer((document, _, _) =>
                {
                    document.Components ??= new OpenApiComponents();

                    document.Servers ??= new List<OpenApiServer>();
                    
                    document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
                    
                    document.Security ??= new List<OpenApiSecurityRequirement>();
                    
                    document.Servers.Clear();
                    document.Servers.Add(new OpenApiServer
                    {
                        Url = builder.Configuration["Swagger:ServerUrl"] ?? "http://localhost:9999",
                        Description = builder.Configuration["Swagger:Description"] ?? "Learn .NET API"
                    });
                    
                    document.Components.SecuritySchemes["Bearer"] =
                        new OpenApiSecurityScheme
                        {
                            Type = SecuritySchemeType.Http,
                            Scheme = "bearer",
                            BearerFormat = "JWT",
                            In = ParameterLocation.Header,
                            Name = "Authorization",
                            Description = "JWT Bearer token"
                        };

                    document.Security.Add(
                        new OpenApiSecurityRequirement
                        {
                            [
                                new OpenApiSecuritySchemeReference(
                                    "Bearer",
                                    document)
                            ] = []
                        });

                    return Task.CompletedTask;
                });
            });

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