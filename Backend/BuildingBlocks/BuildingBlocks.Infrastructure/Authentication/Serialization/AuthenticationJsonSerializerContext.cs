using System.Text.Json.Serialization;
using BuildingBlocks.Shared.DTOs;

namespace BuildingBlocks.Infrastructure.Authentication.Serialization;

[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    WriteIndented = false)]
[JsonSerializable(typeof(BaseResponse<object>))]
internal partial class AuthenticationJsonSerializerContext : JsonSerializerContext;