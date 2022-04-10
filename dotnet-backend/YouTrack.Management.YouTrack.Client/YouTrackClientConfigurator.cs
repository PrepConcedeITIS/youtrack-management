using System;
using System.Net.Http;
using System.Net.Http.Headers;
using YouTrack.Management.Common;

namespace YouTrack.Management.YouTrack.Client
{
    public class YouTrackClientConfigurator : DefaultHttpClientConfigurator
    {
        protected override void ConfigureClient(HttpClient client, BaseClientSettings settings)
        {
            base.ConfigureClient(client, settings);
            if (settings is not YouTrackClientSettings ytSettings)
                throw new InvalidOperationException("Settings should be for YouTrack");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", ytSettings.Token);
            client.DefaultRequestHeaders.CacheControl = CacheControlHeaderValue.Parse(ytSettings.CacheControl);
        }
    }
}