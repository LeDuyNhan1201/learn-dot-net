using System.Text.Json.Serialization;
using BuildingBlocks.Application.Options;
using BuildingBlocks.Shared.DTOs;
using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.API.Serialization;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase, WriteIndented = false)]
[JsonSerializable(typeof(IFormFile))]

[JsonSerializable(typeof(ServerOptions))]
[JsonSerializable(typeof(ErrorResponse))]
internal partial class CommonJsonSerializerContext : JsonSerializerContext
{
}