using System.Collections.Generic;

namespace YouTrack.Management.AssigneeActualize.Entities
{
    public class ProjectsTeamResponse
    {
        public ProjectsTeamResponse(List<ProjectTeam> projectTeams)
        {
            ProjectTeams = projectTeams;
        }
        public List<ProjectTeam> ProjectTeams { get; protected set; }
    }
}