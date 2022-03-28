using Newtonsoft.Json;

namespace YouTrack.Management.Shared.Entities.Issue
{
    public class Project
    {
        public string ShortName { get; set; }
        public string Name { get; set; }
        public string Id { get; set; }

        [JsonProperty("$type")]
        public string Type { get; set; }
    }
}