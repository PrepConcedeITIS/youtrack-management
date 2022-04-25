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

        public AssigneeActualizeController(AssigneeActualizeService assigneeActualizeService,
            YoutrackManagementDbContext dbContext)
        {
            _assigneeActualizeService = assigneeActualizeService;
            _dbContext = dbContext;
        }

        [HttpPost("{projectShortName}")]
        public async Task<IActionResult> UpdateAssignees(string projectShortName)
        {
            await _assigneeActualizeService.Handle(projectShortName);
            return Ok();
        }

        [HttpGet("{projectShortName}")]
        public async Task<ActionResult<List<AssigneeResponse>>> GetAssignees(string projectShortName)
        {
            var result = await _dbContext.Assignees
                .Where(x => x.ProjectName == projectShortName)
                .Select(x => new AssigneeResponse(projectShortName, x.Login))
                .ToListAsync();
            return result;
        }
    }
}