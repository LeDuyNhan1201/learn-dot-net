using System.Text.Json.Serialization;
using BuildingBlocks.Application.Options;
using Domain.Entities;

namespace Restaurant.API.Serialization;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase, WriteIndented = false)]
[JsonSerializable(typeof(Todo[]))]
[JsonSerializable(typeof(List<Todo>))]
[JsonSerializable(typeof(IReadOnlyList<Todo>))]
[JsonSerializable(typeof(Todo))]

[JsonSerializable(typeof(ServerOptions))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}