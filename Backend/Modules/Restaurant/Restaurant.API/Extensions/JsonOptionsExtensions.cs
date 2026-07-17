using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using BuildingBlocks.API.Serialization.Converters;
using BuildingBlocks.API.Serialization.Resolvers;
using Restaurant.API.Serialization;

namespace Restaurant.API.Extensions;

public static class JsonOptionsExtensions
{
    public static IServiceCollection ConfigureApplicationJson(this IServiceCollection services)
    {
        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;

            IList<IJsonTypeInfoResolver> resolvers =
            [
                CommonJsonSerializerContext.Default,
                AppJsonSerializerContext.Default
            ];

            IList<JsonConverter> converters =
            [
                new DateOnlyJsonConverter()
            ];

            options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;

            foreach (var resolver in resolvers) options.SerializerOptions.TypeInfoResolverChain.Insert(0, resolver);
            foreach (var converter in converters) options.SerializerOptions.Converters.Add(converter);
        });

        return services;
    }
}