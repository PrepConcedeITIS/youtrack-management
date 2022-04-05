using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace YouTrack.Management.ApiGateway.Controllers
{
    [Route("[controller]")]
    public class ManageAssigneesController: ControllerBase
    {
        [HttpPost("assign/{sprint}")]
        public async Task<IActionResult> Assign(string sprint)
        {
            return Ok();
        }
    }
}