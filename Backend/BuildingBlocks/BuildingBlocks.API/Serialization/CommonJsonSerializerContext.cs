using System.Text.Json.Serialization;
using BuildingBlocks.Application.Options;
using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.API.Serialization;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase, WriteIndented = false)]
[JsonSerializable(typeof(IFormFile))]

[JsonSerializable(typeof(ServerOptions))]
internal partial class CommonJsonSerializerContext : JsonSerializerContext
{
}