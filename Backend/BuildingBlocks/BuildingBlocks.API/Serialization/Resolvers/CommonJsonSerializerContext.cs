using System.Text.Json.Serialization;
using BuildingBlocks.SharedKernel.DTOs;
using BuildingBlocks.SharedKernel.Options;
using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.API.Serialization.Resolvers;

[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    WriteIndented = false)]
[JsonSerializable(typeof(IFormFile))]
[JsonSerializable(typeof(ServerOptions))]
[JsonSerializable(typeof(BaseResponse<Guid>))]
[JsonSerializable(typeof(BaseResponse<string>))]
[JsonSerializable(typeof(Dictionary<string, string>))]
[JsonSerializable(typeof(IDictionary<string, string>))]
[JsonSerializable(typeof(BaseResponse<Dictionary<string, string>>))]
[JsonSerializable(typeof(BaseResponse<IDictionary<string, string>>))]
[JsonSerializable(typeof(BaseResponse<Dictionary<string, string[]>>))]
[JsonSerializable(typeof(BaseResponse<IDictionary<string, string[]>>))]
public partial class CommonJsonSerializerContext : JsonSerializerContext;