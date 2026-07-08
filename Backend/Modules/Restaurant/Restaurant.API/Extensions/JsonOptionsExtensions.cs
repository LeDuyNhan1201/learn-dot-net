using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using BuildingBlocks.API.Serialization.Converters;
using BuildingBlocks.API.Serialization.Resolvers;
using Restaurant.API.Serialization;

namespace Restaurant.API.Extensions;

/// <summary>
///     Extension methods for configuring JSON serialization options in the application.
/// </summary>
public static class JsonOptionsExtensions
{
    /// <summary>
    ///     Configures JSON serialization options for the application, including custom converters and type info resolvers.
    /// </summary>
    /// <param name="services">The IServiceCollection to which the JSON options will be applied.</param>
    /// <returns>The IServiceCollection with the configured JSON options.</returns>
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