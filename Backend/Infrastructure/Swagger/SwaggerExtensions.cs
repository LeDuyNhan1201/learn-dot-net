using Infrastructure.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Infrastructure.Swagger;

public static class SwaggerExtensions
{
    public static IServiceCollection AddCustomSwagger(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);

        var options = configuration.GetSection(SwaggerOptions.SectionName).Get<SwaggerOptions>() 
                      ?? throw new InvalidOperationException("Swagger configuration is missing.");

        // Register the Swagger generator, defining one or more Swagger documents
        services.AddSwaggerGen(c =>
        {
            c.EnableAnnotations();
            c.SwaggerDoc(options.Version, new OpenApiInfo
            {
                Title = options.Title,
                Version = options.Version,
                Description = options.Description,
                TermsOfService = new Uri("https://swagger.io/terms/"),
                Contact = new OpenApiContact
                {
                    Name = options.ContactName,
                    Email = options.ContactEmail
                },
                License = new OpenApiLicense
                {
                    Name = "Apache 2.0",
                    Url = new Uri("https://www.apache.org/licenses/LICENSE-2.0.html")
                }
            });

            c.AddServer(new OpenApiServer
            {
                Url = options.ServerUrl,
                Description = options.Description
            });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
            });

            c.OperationFilter<MultiPartFileOperationFilter>();
            c.OperationFilter<AuthOperationFilter>();
        });
        
        services.AddEndpointsApiExplorer();
        return services;
    }

    public static IApplicationBuilder UseSwaggerUi(this IApplicationBuilder app, IConfiguration configuration)
    {
        var options = configuration.GetSection(SwaggerOptions.SectionName).Get<SwaggerOptions>() 
                      ?? throw new InvalidOperationException("Swagger configuration is missing.");
        
        app.UseSwagger();

        // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
        app.UseSwaggerUI(swaggerUiOptions =>
        {
            swaggerUiOptions.ConfigObject = new ConfigObject
            {
                ShowCommonExtensions = true
            };
            swaggerUiOptions.SupportedSubmitMethods
            (
                SubmitMethod.Get, 
                SubmitMethod.Post, 
                SubmitMethod.Put, 
                SubmitMethod.Delete, 
                SubmitMethod.Patch);
            
            swaggerUiOptions.SwaggerEndpoint("/swagger/" + options.Version + "/swagger.json", options.ApiDocs);
            //redirect root url to swagger ui
            // swaggerUiOptions.RoutePrefix = configuration["Server:BasePath"];
        });

        return app;
    }
}
