using System.Text.Json.Serialization;
using Infrastructure.Options;
using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.API.Serialization;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase, WriteIndented = false)]
[JsonSerializable(typeof(IFormFile))]

[JsonSerializable(typeof(ServerOptions))]
internal partial class CommonJsonSerializerContext : JsonSerializerContext
{
}