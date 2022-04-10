namespace YouTrack.Management.Common
{
    public class ServiceEndpoint
    {
        public string Url { get; set; }

        public int TimeoutInSeconds { get; set; } = 300;
    }
}