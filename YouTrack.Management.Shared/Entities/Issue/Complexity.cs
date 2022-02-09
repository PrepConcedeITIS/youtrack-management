namespace YouTrack.Management.Shared.Entities.Issue
{
    public class Complexity : HasId<string>
    {
        public string Name { get; set; }

        public Complexity(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}