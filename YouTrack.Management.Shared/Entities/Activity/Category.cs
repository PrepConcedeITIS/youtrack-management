using Newtonsoft.Json;

namespace YouTrack.Management.Shared.Entities.Activity
{
    public class Category : HasId
    {
        [JsonProperty("$type")]
        public string Type { get; set; }

        public Category(string id, string type) : base(id)
        {
            Type = type;
        }
    }
}