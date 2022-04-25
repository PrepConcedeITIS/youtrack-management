using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using MoreLinq.Extensions;
using Newtonsoft.Json;
using YouTrack.Management.AssigneeActualize.Entities;
using YouTrack.Management.RelationalDal;
using YouTrack.Management.Shared.Entities.Issue;

namespace YouTrack.Management.AssigneeActualize
{
    public class AssigneeActualizeService
    {
        private readonly YoutrackManagementDbContext _dbContext;
        private readonly HttpClient _httpClient;

        public AssigneeActualizeService(YoutrackManagementDbContext dbContext, IHttpClientFactory httpClientFactory)
        {
            _dbContext = dbContext;
            _httpClient = httpClientFactory.CreateClient("youtrack-hub");
        }

        public async Task Handle(string projectShortName)
        {
            var assignees = (await GetProjectAssignees(projectShortName)).ToList();
            var existing = _dbContext.Assignees
                .Where(x => x.ProjectName == projectShortName)
                .ToList();
            var toAdd = assignees
                .ExceptBy(existing, assignee => assignee.Id)
                .ToList();
            var toDelete = existing
                .ExceptBy(assignees, assignee => assignee.Id);

            toAdd.ForEach(assignee => assignee.ProjectName = projectShortName);

            await _dbContext.Assignees.AddRangeAsync(toAdd);
            _dbContext.Assignees.RemoveRange(toDelete);

            await _dbContext.SaveChangesAsync();
        }

        private async Task<IEnumerable<Assignee>> GetProjectAssignees(string projectName)
        {
            var httpResponse = await _httpClient.GetAsync(
                "projectteams?fields=project(name,id,key),users(id,login,fullName,email,name,banned)");
            var content = await httpResponse.Content.ReadAsStringAsync();
            var teamsResponse = JsonConvert.DeserializeObject<ProjectsTeamResponse>(content);
            return teamsResponse.ProjectTeams
                .FirstOrDefault(team => team.Project.Key == projectName)?.Users
                .Where(x => x.Login != "root");
        }
    }
}