using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using BuildingBlocks.API.Extensions;
using BuildingBlocks.API.Interfaces;
using BuildingBlocks.API.Middlewares;
using BuildingBlocks.API.Serialization.Converters;
using BuildingBlocks.API.Serialization.Resolvers;
using BuildingBlocks.Infrastructure.Authentication.Extensions;
using BuildingBlocks.Infrastructure.Observability.Extensions;
using BuildingBlocks.Infrastructure.OpenApi.Extensions;
using FluentValidation;
using Microsoft.IdentityModel.Logging;
using Restaurant.API.Endpoints;
using Restaurant.API.Serialization;
using Restaurant.Application.DTOs;
using Restaurant.Application.Services;
using Restaurant.Application.Services.Interfaces;
using Restaurant.Application.Validation.Validators;

var builder = WebApplication.CreateSlimBuilder(args);

IdentityModelEventSource.ShowPII = true;

builder.Services.AddBaseOptions();
builder.Services.AddI18NLocalization();

if (!builder.Environment.IsEnvironment("Local")) builder.Services.AddObservability(builder.Configuration);
builder.Services.AddScalarOpenApi();

if (builder.Environment.IsEnvironment("Local"))
{
    builder.Services.AddValidatorsFromAssemblyContaining<MenuItemValidator>();
}
else
{
    builder.Services.AddScoped<IValidator<MenuItemDto>, MenuItemValidator>();
    builder.Services.AddScoped<MenuItemValidator>();
}

builder.AddAuthenticationWithAuthorization();

builder.Services.AddScoped<ITodoService, TodoService>();

builder.Services.ConfigureHttpJsonOptions(options =>
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

    foreach (var resolver in resolvers) options.SerializerOptions.TypeInfoResolverChain.Insert(0, resolver);
    foreach (var converter in converters) options.SerializerOptions.Converters.Add(converter);
});

var app = builder.Build();

IEndpointModule[] restEndpoints =
[
    new HealthEndpointsV1(),
    new HealthEndpointsV2(),
    new TodoEndpointsV1(),
    new MenuItemEndpointsV1()
];

app.UsePathBase(app.Configuration["Server:BasePath"]);
app.UseAppLocalization();
app.UseMetricsExporter(app.Configuration);

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<IdentityContextMiddleware>();

app.UseRestRouting(restEndpoints);

app.Run();