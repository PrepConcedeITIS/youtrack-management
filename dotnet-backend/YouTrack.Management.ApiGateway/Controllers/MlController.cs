using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using YouTrack.Management.MachineLearning.Client;
using YouTrack.Management.ResolvedIssues.Client;

namespace YouTrack.Management.ApiGateway.Controllers
{
    [Route("[controller]")]
    public class MlController: ControllerBase
    {
        private readonly ResolvedIssuesClient _resolvedIssuesClient;
        private readonly MachineLearningClient _machineLearningClient;

        public MlController(ResolvedIssuesClient resolvedIssuesClient, MachineLearningClient machineLearningClient)
        {
            _resolvedIssuesClient = resolvedIssuesClient;
            _machineLearningClient = machineLearningClient;
        }

        [HttpPost("train")]
        public async Task<IActionResult> TrainModel([FromQuery] bool withMock)
        {
            var resolvedIssues = await _resolvedIssuesClient.GetIssuesMlCsv(withMock);
            var trainResult = await _machineLearningClient.TrainModel(resolvedIssues);
            return Ok();
        }
    }
}