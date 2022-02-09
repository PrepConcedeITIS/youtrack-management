using System.Collections.Generic;

namespace YouTrack.Management.Shared.Entities.Issue
{
    public class IssueLink
    {
        public class IssueLinkType
        {
            public string Name { get; set; }
        }

        public enum LinkDirection
        {
            OUTWARD,
            INWARD,
            BOTH
        }

        public IssueLinkType LinkType { get; set; }
        public List<Issue> Issues { get; set; }
        public LinkDirection Direction { get; set; }
    }
}