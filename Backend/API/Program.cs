using API.Extensions;

var builder = WebApplication
    .CreateBuilder(args)
    .AddApplicationServices();

var app = builder
    .Build()
    .UseApplicationPipeline();

app.Run();