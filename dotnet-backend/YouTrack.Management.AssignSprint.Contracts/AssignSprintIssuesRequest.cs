namespace YouTrack.Management.AssignSprint.Contracts
{
    public class AssignSprintIssuesRequest
    {
        public AssignSprintIssuesRequest()
        {
        }

        public AssignSprintIssuesRequest(string sprintName, string projectShortName)
        {
            SprintName = sprintName;
            ProjectShortName = projectShortName;
        }

        public string SprintName { get; set; }
        public string ProjectShortName { get; set; }
    }
}