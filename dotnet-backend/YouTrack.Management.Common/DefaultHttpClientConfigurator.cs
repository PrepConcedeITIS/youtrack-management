using System;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;

namespace YouTrack.Management.Common
{
    public class DefaultHttpClientConfigurator : IHttpClientConfigurator
    {
        /// <summary>
        /// Настроить клиента
        /// </summary>
        protected virtual void ConfigureClient(HttpClient client, BaseClientSettings settings)
        {
            client.Timeout = TimeSpan.FromSeconds(settings.Endpoint.TimeoutInSeconds);
            //client.DefaultRequestVersion = HttpVersion.Version20;
            client.BaseAddress = new Uri(settings.Endpoint.Url);
        }

        /// <summary>
        /// Настройка http handler
        /// </summary>
        protected virtual HttpClientHandler CreateClientHandler(BaseClientSettings settings)
        {
            var httpHandler = new HttpClientHandler();
            httpHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
            return httpHandler;
        }

        /// <summary>
        /// Регистрация клиента в зависимости
        /// </summary>
        public virtual void AddClient<TClient>(IServiceCollection services, BaseClientSettings settings)
            where TClient : BaseClient
        {
            services.AddHttpClient<TClient>(client => ConfigureClient(client, settings))
                .ConfigurePrimaryHttpMessageHandler(() =>
                    CreateClientHandler(settings));
        }
    }
}