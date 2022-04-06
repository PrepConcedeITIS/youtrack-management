using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace YouTrack.Management.ApiGateway.Controllers
{
    [Route("[controller]")]
    public class MlController: ControllerBase
    {
        [HttpPost("train")]
        public async Task<IActionResult> Assign(string sprint)
        {
            return Ok();
        }
    }
}