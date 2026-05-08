using System.Text.Json.Serialization;
using API.Options;
using Domain.Entities;

namespace API.Serialization;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase, WriteIndented = false)]
[JsonSerializable(typeof(Todo[]))]
[JsonSerializable(typeof(List<Todo>))]
[JsonSerializable(typeof(IReadOnlyList<Todo>))]
[JsonSerializable(typeof(Todo))]

[JsonSerializable(typeof(AppOptions))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}