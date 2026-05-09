using API.Extensions;

var builder = WebApplication.CreateSlimBuilder(args);

builder.AddApplicationServices();

var app = builder.Build();

app.UseApplicationPipeline();

app.Run();