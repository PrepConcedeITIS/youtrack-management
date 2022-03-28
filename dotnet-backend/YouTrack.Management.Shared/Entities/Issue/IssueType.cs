namespace YouTrack.Management.Shared.Entities.Issue
{
    public class IssueType : HasId<string>
    {
        public string Name { get; set; }

        public IssueType(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}