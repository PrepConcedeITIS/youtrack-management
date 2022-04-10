using Microsoft.Extensions.DependencyInjection;

namespace YouTrack.Management.Common
{
    public interface IHttpClientConfigurator
    {
        void AddClient<TClient>(IServiceCollection services, BaseClientSettings settings)
            where TClient : BaseClient;
    }
}