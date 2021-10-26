using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AppMetrica.PostAPI;

public static class ContainerExtensions
{
    public static IServiceCollection AddAppMetricaReporter(this IServiceCollection services, IConfigurationSection? configurationSection = null, Action<AppMetricaOptions>? configureOptions = null)
    {
        var optionsBuilder = services.AddOptions<AppMetricaOptions>();

        if (configurationSection != null)
            optionsBuilder.Bind(configurationSection);
        if (configureOptions != null)
            optionsBuilder.Configure(configureOptions);

        optionsBuilder
            .Validate(opt => opt.ApplicationId != default, $"{nameof(AppMetricaOptions.ApplicationId)} must be set.")
            .Validate(opt => opt.PostApiKey != default, $"{nameof(AppMetricaOptions.PostApiKey)} must be set.");

        services.AddHttpClient<IAppMetricaUploader, AppMetricaUploader>(httpClient =>
        {
            httpClient.BaseAddress = AppMetricaOptions.BaseUrl;
        });

        return services;
    }
}
