using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace YouTrack.Management.AssigneeActualize.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AssigneeActualizeController : ControllerBase
    {
        private readonly AssigneeActualizeService _assigneeActualizeService;
        public AssigneeActualizeController(AssigneeActualizeService assigneeActualizeService)
        {
            _assigneeActualizeService = assigneeActualizeService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            await _assigneeActualizeService.Handle();
            return Ok();
        }
    }
}