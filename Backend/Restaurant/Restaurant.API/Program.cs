using Restaurant.API.Extensions;

var builder = WebApplication.CreateSlimBuilder(args);

builder.ConfigureServices();

var app = builder.Build();

await app.InitializeAsync();

app.ConfigurePipeline();

app.Run();