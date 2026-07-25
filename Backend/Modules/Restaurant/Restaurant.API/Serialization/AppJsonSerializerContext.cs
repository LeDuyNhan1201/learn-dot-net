using System.Text.Json.Serialization;
using BuildingBlocks.SharedKernel.Options;
using Restaurant.API.Endpoints;
using Restaurant.Application.DTOs;
using Restaurant.Domain.Entities;

namespace Restaurant.API.Serialization;

[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    WriteIndented = false,
    UseStringEnumConverter = true)]
[JsonSerializable(typeof(Todo[]))]
[JsonSerializable(typeof(List<Todo>))]
[JsonSerializable(typeof(IReadOnlyList<Todo>))]
[JsonSerializable(typeof(Todo))]
[JsonSerializable(typeof(ServerOptions))]
[JsonSerializable(typeof(CreateMenuItemRequest))]
[JsonSerializable(typeof(TokenRequest))]
[JsonSerializable(typeof(TokenResponse))]
internal partial class AppJsonSerializerContext : JsonSerializerContext;