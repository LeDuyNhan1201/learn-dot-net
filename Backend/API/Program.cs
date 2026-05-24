using API.Endpoints;
using API.Serialization;
using Application.Services;
using BuildingBlocks.API.Extensions;

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