using Newtonsoft.Json;

namespace YouTrack.Management.Shared.Entities
{
    public class StateChangedElement : HasId
    {
        public string Name { get; set; }

        [JsonProperty("$type")]
        public string Type { get; set; }

        public StateChangedElement(string id, string name, string type) : base(id)
        {
            Name = name;
            Type = type;
        }
    }
}