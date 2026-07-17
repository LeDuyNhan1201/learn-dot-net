using System.Text.Json.Serialization;
using BuildingBlocks.SharedKernel.DTOs;

namespace BuildingBlocks.Identity.Serialization;

[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    WriteIndented = false)]
[JsonSerializable(typeof(BaseResponse<object>))]
internal partial class AuthenticationJsonSerializerContext : JsonSerializerContext;