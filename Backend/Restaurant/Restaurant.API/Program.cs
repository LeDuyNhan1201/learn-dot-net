using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using BuildingBlocks.API.Extensions;
using BuildingBlocks.API.Interfaces;
using BuildingBlocks.API.Middlewares;
using BuildingBlocks.API.Serialization.Converters;
using BuildingBlocks.Infrastructure.Observability.Extensions;
using Restaurant.API.Endpoints;
using Restaurant.Application.Services;
using Restaurant.Application.Services.Interfaces;
using AppJsonSerializerContext = Restaurant.API.Serialization.AppJsonSerializerContext;
using CommonJsonSerializerContext = BuildingBlocks.API.Serialization.Resolvers.CommonJsonSerializerContext;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddI18NLocalization()
    .AddBaseServices(builder.Configuration);

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

app
    .UseAppLocalization()
    .UseMetricsExporter(app.Configuration);

IEndpointModule[] restEndpoints =
[
    new HealthEndpointsV1(),
    new HealthEndpointsV2(),
    new TodoEndpointsV1()
];

app.UseRestRouting(restEndpoints);

app
    .UseAuthentication()
    .UseMiddleware<IdentityContextMiddleware>()
    .UseAuthorization();

app.Run();