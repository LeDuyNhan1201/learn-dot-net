using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using BuildingBlocks.API.Extensions;
using BuildingBlocks.API.Interfaces;
using BuildingBlocks.API.Middlewares;
using BuildingBlocks.API.Serialization.Converters;
using BuildingBlocks.API.Serialization.Resolvers;
using BuildingBlocks.Domain.Exceptions.Handlers;
using BuildingBlocks.Domain.Validation;
using BuildingBlocks.Infrastructure.Authentication.Extensions;
using BuildingBlocks.Infrastructure.Observability.Extensions;
using BuildingBlocks.Infrastructure.OpenApi.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.IdentityModel.Logging;
using Restaurant.API;
using Restaurant.API.Endpoints;
using Restaurant.API.Serialization;
using Restaurant.Application.Contracts;
using Restaurant.Application.DTOs;
using Restaurant.Application.Handlers;
using Restaurant.Application.Services;
using Restaurant.Application.Services.Interfaces;
using Restaurant.Application.Validation.Validators;

var builder = WebApplication.CreateSlimBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;
var environment = builder.Environment;

IdentityModelEventSource.ShowPII = true;

if (builder.Environment.IsDevelopment())
{
    builder.Host.UseDefaultServiceProvider(options =>
    {
        options.ValidateOnBuild = false;
        options.ValidateScopes = false;
    });
}

services.AddBaseOptions();
services.AddI18NLocalization();
services.AddScalarOpenApi();

if (environment.IsEnvironment("Local"))
{
    services.AddValidatorsFromAssemblyContaining<IMenuItemValidator.CreateRequest>();
}
else
{
    services.AddObservability(builder.Configuration);
    services.AddScoped<IValidator<IMenuItemDto.CreateRequest>, IMenuItemValidator.CreateRequest>();
    services.AddScoped<IMenuItemValidator.CreateRequest>();
}

// services.AddScoped(typeof(ValidationFilter<,>));
// services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblyContaining<IMenuItemCommand>();
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
});

services.AddAuthenticationWithAuthorization(configuration, environment);

services.AddScoped<ITodoService, TodoService>();

services.ConfigureHttpJsonOptions(options =>
{
    IList<IJsonTypeInfoResolver> resolvers =
    [
        CommonJsonSerializerContext.Default,
        AppJsonSerializerContext.Default
    ];

    IList<JsonConverter> converters =
    [
        new DateOnlyJsonConverter()
    ];

    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;

    foreach (var resolver in resolvers) options.SerializerOptions.TypeInfoResolverChain.Insert(0, resolver);
    foreach (var converter in converters) options.SerializerOptions.Converters.Add(converter);
});

services.AddExceptionHandler<GlobalExceptionHandler<Messages>>();
services.AddProblemDetails();

var app = builder.Build();
var appConfiguration = app.Configuration;

app.MapTokenEndpoints();

IEndpointModule[] restEndpoints =
[
    new HealthEndpointsV1(),
    new HealthEndpointsV2(),
    new TodoEndpointsV1(),
    new MenuItemEndpointsV1(),
    new MenuItemEndpointsV2()
];

app.UsePathBase(appConfiguration["Server:BasePath"]);
app.UseAppLocalization();
app.UseMetricsExporter(appConfiguration);

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<IdentityContextMiddleware>();

app.UseExceptionHandler();

app.UseRestRouting(restEndpoints);

app.Run();
