using System.Text.Json.Serialization;
using BuildingBlocks.Shared.Options;
using Restaurant.Application.DTOs;
using Restaurant.Domain.Entities;

namespace Restaurant.API.Serialization;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase, WriteIndented = false)]
[JsonSerializable(typeof(Todo[]))]
[JsonSerializable(typeof(List<Todo>))]
[JsonSerializable(typeof(IReadOnlyList<Todo>))]
[JsonSerializable(typeof(Todo))]
[JsonSerializable(typeof(ServerOptions))]
[JsonSerializable(typeof(MenuItemDto))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}