using YouTrack.Management.Common;

namespace YouTrack.Management.YouTrack.Client
{
    public class YouTrackClientSettings : BaseClientSettings
    {
        public string Token { get; set; }
        public string CacheControl { get; set; }
    }
}