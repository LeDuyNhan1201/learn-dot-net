using API;
using API.Configurations;
using API.Endpoints;
using API.Serialization;
using API.Serialization.Converter;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
    options.SerializerOptions.Converters.Add(new DateOnlyJsonConverter());
});

builder.Services.AddAppLocalization();

builder.Services.AddConfigurations(builder.Configuration);

builder.Services.AddApiServices();

builder.Services.AddOpenApi();

var app = builder.Build();

app.UseAppLocalization();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapTodoEndpoints();
app.MapHealthEndpoints();

app.Run();