using System.Collections.Generic;
using YouTrack.Management.Shared.Entities.Issue;

namespace YouTrack.Management.AssigneeActualize.Entities
{
    public class ProjectTeam
    {
        public ProjectTeam(Project project, List<Assignee> users)
        {
            Project = project;
            Users = users;
        }
        public Project Project { get; protected set; }
        public List<Assignee> Users { get; protected set; }
    }
}