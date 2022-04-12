using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using YouTrack.Management.AssignSprint.Client;
using YouTrack.Management.AssignSprint.Contracts;

namespace YouTrack.Management.ApiGateway.Controllers
{
    [Route("[controller]")]
    public class AssignSprintController : ControllerBase
    {
        private readonly AssignSprintClient _assignSprintClient;

        public AssignSprintController(AssignSprintClient assignSprintClient)
        {
            _assignSprintClient = assignSprintClient;
        }

        [HttpPost]
        public async Task<IActionResult> AssignSprintIssues([FromBody]AssignSprintIssuesRequest request)
        {
            await _assignSprintClient.AssignIssuesToSprint(request);
            return Ok();
        }
    }
}