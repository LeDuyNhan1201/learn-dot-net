using Microsoft.Extensions.Localization;

namespace BuildingBlocks.Shared.Localization;

public sealed class CompositeLocalizer<TLocal>(
    IStringLocalizer<CommonMessages> shared,
    IStringLocalizer<TLocal> local) where TLocal : class
{
    public string this[string key] => Get(key);

    public string this[string key, params object[] arguments] => Get(key, arguments);

    private string Get(string key)
    {
        var localValue = local[key];

        if (!localValue.ResourceNotFound)
        {
            return localValue.Value;
        }

        return shared[key];
    }

    private string Get(
        string key,
        params object[] arguments)
    {
        var localValue = local[key, arguments];

        if (!localValue.ResourceNotFound)
        {
            return localValue.Value;
        }

        return shared[key, arguments];
    }
}