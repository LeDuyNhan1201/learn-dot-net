using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace BuildingBlocks.API.Serialization;

public sealed class SerializationBuilder
{
    internal List<IJsonTypeInfoResolver> Resolvers { get; } = [];

    internal List<JsonConverter> Converters { get; } = [];

    public SerializationBuilder AddContext(IJsonTypeInfoResolver resolver)
    {
        Resolvers.Add(resolver);
        return this;
    }

    public SerializationBuilder AddConverter(JsonConverter converter)
    {
        Converters.Add(converter);
        return this;
    }
}