using System.Collections.Generic;

namespace YouTrack.Management.Shared.Entities
{
    // ReSharper disable once InconsistentNaming
    public class IssueML
    {
        public double AssigneeCorrect { get; set; }
        public List<string> Tags { get; set; }
        public string ProjectName { get; set; }
        //public int EstimateMinutes { get; set; }
        //public int SpentMinutes { get; set; }
        public string AssigneeLogin { get; set; }
        public string Complexity { get; set; }
        //public StateChangelog Changelog { get; set; }
    }
}