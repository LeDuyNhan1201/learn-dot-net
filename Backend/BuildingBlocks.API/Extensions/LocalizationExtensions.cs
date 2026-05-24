using System.Globalization;
using BuildingBlocks.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace BuildingBlocks.API.Extensions;

public static class LocalizationExtensions
{
    public static IServiceCollection AddI18NLocalization(this IServiceCollection services)
    {
        services.AddLocalization(options =>
        {
            options.ResourcesPath = "Resources";
        });

        CultureInfo[] supportedCultures =
        [
            new("en-US"),
            new("vi-VN")
        ];

        services.Configure<RequestLocalizationOptions>(options =>
        {
            options.DefaultRequestCulture = new RequestCulture("en-US");
            options.SupportedCultures = supportedCultures;
            options.SupportedUICultures = supportedCultures;
            options.RequestCultureProviders.Insert(0, new AcceptLanguageHeaderRequestCultureProvider());
        });
        
        services.AddScoped(typeof(CompositeLocalizer<>));

        return services;
    }

    public static IApplicationBuilder UseAppLocalization(this IApplicationBuilder app)
    {
        var options = app.ApplicationServices.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value;
        app.UseRequestLocalization(options);
        return app;
    }
}