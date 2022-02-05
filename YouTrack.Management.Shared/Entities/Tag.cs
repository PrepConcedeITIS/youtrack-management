using Newtonsoft.Json;

namespace YouTrack.Management.Shared.Entities
{
    public class Tag
    {
        public string Name { get; set; }

        [JsonProperty("$type")]
        public string Type { get; set; }
    }
}