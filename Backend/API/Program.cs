using API.Extensions;

var builder = WebApplication
    .CreateBuilder(args)
    .AddCoreComponents();

var app = builder
    .Build()
    .UseApplicationPipeline();

app.Run();