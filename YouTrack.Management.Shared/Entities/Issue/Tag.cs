using Newtonsoft.Json;

namespace YouTrack.Management.Shared.Entities.Issue
{
    public class Tag
    {
        public string Name { get; set; }

        [JsonProperty("$type")]
        public string Type { get; set; }
    }
}