using System.Text.Json.Serialization;
using Domain.Entities;
using Infrastructure.Options;

namespace API.Serialization;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase, WriteIndented = false)]
[JsonSerializable(typeof(Todo[]))]
[JsonSerializable(typeof(List<Todo>))]
[JsonSerializable(typeof(IReadOnlyList<Todo>))]
[JsonSerializable(typeof(Todo))]
[JsonSerializable(typeof(IFormFile))]

[JsonSerializable(typeof(ServerOptions))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}