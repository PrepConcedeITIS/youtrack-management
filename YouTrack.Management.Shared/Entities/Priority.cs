namespace YouTrack.Management.Shared.Entities
{
    public class Priority : HasId<string>
    {
        public string Name { get; set; }

        public Priority(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}