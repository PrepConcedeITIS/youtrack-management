namespace YouTrack.Management.AssigneeActualize.Contracts
{
    public class AssigneeResponse
    {
        public AssigneeResponse()
        {
        }

        public AssigneeResponse(string projectShortName, string login)
        {
            ProjectShortName = projectShortName;
            Login = login;
        }
        public string ProjectShortName { get; set; }
        public string Login { get; set; }
    }
}