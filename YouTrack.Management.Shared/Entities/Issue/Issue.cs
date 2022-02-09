using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using YouTrack.Management.Shared.Entities.Activity;

namespace YouTrack.Management.Shared.Entities.Issue
{
    public class Issue : HasId
    {
        public List<Tag> Tags { get; set; }
        public List<IssueLink> Links { get; set; }
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
        public SuccessGrade SuccessGrade { get; set; }
        public StateChangelog Changelog { get; set; }
        public Priority Priority { get; set; }
        public IssueType IssueType { get; set; }

        [NotMapped]
        public double? EstimationError => (Spent?.Minutes - Estimate?.Minutes) / Estimate?.Minutes;
        
        public Issue(string id, List<Tag> tags,List<IssueLink> links, string summary, string idReadable, Project project) : base(id)
        {
            Tags = tags;
            Links = links;
            Summary = summary;
            IdReadable = idReadable;
            Project = project;
        }
    }
}