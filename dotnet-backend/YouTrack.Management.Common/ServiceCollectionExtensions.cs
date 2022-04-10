using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace YouTrack.Management.Common
{
    public static class ServiceCollectionExtensions
    {
        public static void AddClient<TClient, TClientSettings>(
            this IServiceCollection services,
            IConfiguration configuration,
            IHttpClientConfigurator configurator)
            where TClient : BaseClient
            where TClientSettings : BaseClientSettings, new()
        {
            var settings = configuration.GetSettings<TClientSettings>(typeof(TClient).Name);
            services.AddScoped<TClient>();
            configurator.AddClient<TClient>(services, settings);
        }
    }
}