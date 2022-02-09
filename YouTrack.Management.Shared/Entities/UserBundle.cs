using System.Collections.Generic;
using Newtonsoft.Json;
using YouTrack.Management.Shared.Entities.Issue;

namespace YouTrack.Management.Shared.Entities
{
    public class UserBundle : HasId
    {
        public UserBundle(string id) : base(id)
        {
        }

        public List<Assignee> AggregatedUsers { get; set; }
        public bool IsUpdatable { get; set; }
        public string Name { get; set; }

        [JsonProperty("$type")]
        public string Type { get; set; }
    }
}