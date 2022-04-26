using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using YouTrack.Management.ModelRetrain.EF;
using YouTrack.Management.ModelRetrain.Entities;

namespace YouTrack.Management.ModelRetrain.Controllers
{
    [Route("[controller]")]
    public class RetrainController : ControllerBase
    {
        private readonly RetrainDbContext _dbContext;

        public RetrainController(RetrainDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost("ChangeStatus/{projectShortName}")]
        public async Task<IActionResult> ChangeStatus(string projectShortName)
        {
            var entity = _dbContext.Projects.FirstOrDefault(x => x.ProjectKey == projectShortName);
            if (entity is null)
            {
                await _dbContext.Projects.AddAsync(new Project(projectShortName, true));
            }
            else
            {
                entity.RetrainEnabled = !entity.RetrainEnabled;
                _dbContext.Projects.Update(entity);
            }

            await _dbContext.SaveChangesAsync();
            return Ok(entity?.RetrainEnabled ?? true);
        }
    }
}