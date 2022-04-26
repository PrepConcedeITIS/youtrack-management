namespace YouTrack.Management.ModelRetrain.Entities
{
    public class Project
    {
        protected Project()
        {
        }

        public Project(string projectKey, bool retrainEnabled)
        {
            ProjectKey = projectKey;
            RetrainEnabled = retrainEnabled;
        }

        public int Id { get; set; }
        public string ProjectKey { get; set; }
        public bool RetrainEnabled { get; set; }
    }
}