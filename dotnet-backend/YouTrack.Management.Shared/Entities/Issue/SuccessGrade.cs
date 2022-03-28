namespace YouTrack.Management.Shared.Entities.Issue
{
    public class SuccessGrade : HasId<string>
    {
        public string Name { get; set; }

        public SuccessGrade(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}