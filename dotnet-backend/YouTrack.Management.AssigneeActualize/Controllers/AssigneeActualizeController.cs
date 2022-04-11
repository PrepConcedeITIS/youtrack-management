using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YouTrack.Management.AssigneeActualize.Contracts;
using YouTrack.Management.RelationalDal;

namespace YouTrack.Management.AssigneeActualize.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AssigneeActualizeController : ControllerBase
    {
        private readonly AssigneeActualizeService _assigneeActualizeService;
        private readonly YoutrackManagementDbContext _dbContext;

        public AssigneeActualizeController(AssigneeActualizeService assigneeActualizeService, YoutrackManagementDbContext dbContext)
        {
            _assigneeActualizeService = assigneeActualizeService;
            _dbContext = dbContext;
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAssignees()
        {
            //todo: by project
            await _assigneeActualizeService.Handle();
            return Ok();
        }

        [HttpGet("{projectShortName}")]
        public async Task<ActionResult<List<AssigneeResponse>>> GetAssignees(string projectShortName)
        {
            var result = await _dbContext.Assignees
                //todo: .Where(x=>x.ProjectName == projectShortName)
                .Select(x => new AssigneeResponse("AVG", x.Login))
                .ToListAsync();
            return result;
        }
    }
}