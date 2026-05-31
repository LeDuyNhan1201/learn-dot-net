using System.Text.Json.Serialization;
using BuildingBlocks.Shared.DTOs;
using BuildingBlocks.Shared.Options;
using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.API.Serialization.Resolvers;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase, WriteIndented = false)]
[JsonSerializable(typeof(IFormFile))]
[JsonSerializable(typeof(ServerOptions))]
[JsonSerializable(typeof(ErrorResponse))]
public partial class CommonJsonSerializerContext : JsonSerializerContext;