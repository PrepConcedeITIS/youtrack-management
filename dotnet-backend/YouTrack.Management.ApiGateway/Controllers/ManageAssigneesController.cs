using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using YouTrack.Management.AssigneeActualize.Client;

namespace YouTrack.Management.ApiGateway.Controllers
{
    [Route("[controller]")]
    public class ManageAssigneesController : ControllerBase
    {
        private readonly AssigneeActualizeClient _assigneeActualizeClient;

        public ManageAssigneesController(AssigneeActualizeClient assigneeActualizeClient)
        {
            _assigneeActualizeClient = assigneeActualizeClient;
        }

        [HttpPost("assign/{sprint}")]
        public async Task<IActionResult> Assign(string sprint)
        {
            return Ok();
        }

        [HttpPost("actualizeProjectAssignees/{projectName}")]
        public async Task<IActionResult> ActualizeProjectAssignees(string projectName)
        {
            await _assigneeActualizeClient.ActualizeAssigneesInDatabase();
            return Ok();
        }
    }
}