using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using YouTrack.Management.ResolvedIssues.Services;

namespace YouTrack.Management.ResolvedIssues.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IssuesController : ControllerBase
    {
        private readonly IIssueLoader _issueLoader;

        public IssuesController(IIssueLoader issueLoader)
        {
            _issueLoader = issueLoader;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            await _issueLoader.Get();
            return Ok();
        }
    }
}