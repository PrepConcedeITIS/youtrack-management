using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using YouTrack.Management.MachineLearning.Client;
using YouTrack.Management.MachineLearning.Contracts.Requests;
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

        [HttpPost("Train")]
        public async Task<IActionResult> TrainModel([FromBody] TrainModelRequest request)
        {
            var resolvedIssues = await _resolvedIssuesClient.GetIssuesMlCsv(request.WithMock);
            var trainResult = await _machineLearningClient.TrainModel(resolvedIssues);
            return Ok();
        }
    }
}