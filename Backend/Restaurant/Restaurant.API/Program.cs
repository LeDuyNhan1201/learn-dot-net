using Application.Services;
using BuildingBlocks.API.Extensions;
using Restaurant.API.Endpoints;
using AppJsonSerializerContext = Restaurant.API.Serialization.AppJsonSerializerContext;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddOptionsConfiguration(builder.Configuration)
    .AddI18NLocalization()
    .AddInfrastructure(builder.Configuration)
    .AddApiServices(new ServiceModule())
    .AddSerializations(options =>
    {
        options.AddContext(AppJsonSerializerContext.Default);

        // options.AddConverter(new MoneyJsonConverter());
    });

var app = builder.Build();

app.UseApplicationPipeline(
    new HealthEndpointsV1(),
    new HealthEndpointsV2(),
    new TodoEndpointsV1());

app.Run();