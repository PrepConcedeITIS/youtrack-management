namespace YouTrack.Management.AssignSprint.Contracts
{
    public class AssignSprintIssuesRequest
    {
        public AssignSprintIssuesRequest()
        {
        }

        public AssignSprintIssuesRequest(string sprintName, string project)
        {
            SprintName = sprintName;
            Project = project;
        }

        public string SprintName { get; set; }
        public string Project { get; set; }
    }
}