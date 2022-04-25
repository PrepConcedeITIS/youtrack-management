using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using YouTrack.Management.AssignSprint.Client;
using YouTrack.Management.AssignSprint.Contracts;
using YouTrack.Management.YouTrack.Client;

namespace YouTrack.Management.ApiGateway.Controllers
{
    [Route("[controller]")]
    public class SprintController : ControllerBase
    {
        private readonly AssignSprintClient _assignSprintClient;
        private readonly YouTrackClient _youTrackClient;

        public SprintController(AssignSprintClient assignSprintClient, YouTrackClient youTrackClient)
        {
            _assignSprintClient = assignSprintClient;
            _youTrackClient = youTrackClient;
        }

        [HttpPost("AssignIssues")]
        public async Task<IActionResult> AssignSprintIssues([FromBody]AssignSprintIssuesRequest request)
        {
            await _assignSprintClient.AssignIssuesToSprint(request);
            return Ok();
        }
        [HttpPost("DiscardAssignees")]
        public async Task<IActionResult> UnAssignIssues([FromBody] AssignSprintIssuesRequest request)
        {
            var issues = await _youTrackClient.GetIssuesBySprint(projectShortName: request.ProjectShortName);
            var tasks = issues.Select(x => _youTrackClient.UnAssignIssue(x.IdReadable));
            await Task.WhenAll(tasks);
            return Ok();
        }
    }
}