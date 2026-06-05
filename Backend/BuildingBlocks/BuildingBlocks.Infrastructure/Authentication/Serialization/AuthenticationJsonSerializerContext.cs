using System.Text.Json.Serialization;
using BuildingBlocks.Shared.DTOs;

namespace BuildingBlocks.Infrastructure.Authentication.Serialization;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase, WriteIndented = false)]
[JsonSerializable(typeof(ErrorResponse))]
internal partial class AuthenticationJsonSerializerContext : JsonSerializerContext;
