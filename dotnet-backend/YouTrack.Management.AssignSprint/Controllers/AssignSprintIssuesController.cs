using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using YouTrack.Management.AssignSprint.Contracts;

namespace YouTrack.Management.AssignSprint.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AssignSprintIssuesController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Assign(AssignSprintIssuesRequest request)
        {

            return Ok();
        }
    }
}