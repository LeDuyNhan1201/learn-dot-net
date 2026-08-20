using System.Text.Json.Nodes;

namespace BuildingBlocks.OpenApi.Abstractions;

public interface ISchemaExampleProvider
{
    Type TargetType { get; }
    JsonNode GetExample();
}