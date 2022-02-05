using System.Collections.Generic;
using Newtonsoft.Json;

namespace YouTrack.Management.Shared.Entities
{
    public class Issue : HasId
    {
        public List<Tag> Tags { get; set; }
        public string Summary { get; set; }
        public string IdReadable { get; set; }

        public Project Project { get; set; }

        //[JsonIgnore]
        public List<CustomField> CustomFields { get; set; }

        [JsonProperty("$type")]
        public string Type { get; set; }

        public Estimate Estimate { get; set; }
        public Spent Spent { get; set; }
        public Assignee Assignee { get; set; }
        public State State { get; set; }
        public Complexity Complexity { get; set; }
        public StateChangelog Changelog { get; set; }
        public Priority Priority { get; set; }

        public Issue(string id, List<Tag> tags, string summary, string idReadable, Project project) : base(id)
        {
            Tags = tags;
            Summary = summary;
            IdReadable = idReadable;
            Project = project;
        }
    }
}