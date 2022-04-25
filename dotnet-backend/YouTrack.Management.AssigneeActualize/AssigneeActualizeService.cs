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

        public async Task Handle()
        {
            var assignees = (await GetProjectAssignees("Average project")).ToList();
            //todo: by project
            var existing = _dbContext.Assignees.Where(x => true).ToList();
            var toAdd = assignees.ExceptBy(existing, assignee => assignee.Id);
            var toDelete = existing.ExceptBy(assignees, assignee => assignee.Id);
            await _dbContext.Assignees.AddRangeAsync(toAdd);
            _dbContext.Assignees.RemoveRange(toDelete);
            await _dbContext.SaveChangesAsync();
        }

        private async Task<IEnumerable<Assignee>> GetProjectAssignees(string projectName)
        {
            var httpResponse = await _httpClient.GetAsync(
                "projectteams?fields=project(name,id),users(id,login,fullName,email,name,banned)");
            var content = await httpResponse.Content.ReadAsStringAsync();
            var teamsResponse = JsonConvert.DeserializeObject<ProjectsTeamResponse>(content);
            return teamsResponse.ProjectTeams
                .FirstOrDefault(team => team.Project.Name == projectName)?.Users
                .Where(x => x.Login != "root");
        }
    }
}