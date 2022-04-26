using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using YouTrack.Management.MachineLearning.Client;
using YouTrack.Management.MachineLearning.Contracts.Requests;
using YouTrack.Management.ModelRetrain.Client;
using YouTrack.Management.ResolvedIssues.Client;

namespace YouTrack.Management.ApiGateway.Controllers
{
    [Route("[controller]")]
    public class MlController : ControllerBase
    {
        private readonly ResolvedIssuesClient _resolvedIssuesClient;
        private readonly MachineLearningClient _machineLearningClient;
        private readonly ModelRetrainClient _modelRetrainClient;

        public MlController(ResolvedIssuesClient resolvedIssuesClient, MachineLearningClient machineLearningClient,
            ModelRetrainClient modelRetrainClient)
        {
            _resolvedIssuesClient = resolvedIssuesClient;
            _machineLearningClient = machineLearningClient;
            _modelRetrainClient = modelRetrainClient;
        }

        [HttpPost("Train")]
        public async Task<IActionResult> TrainModel([FromBody] TrainModelRequest request)
        {
            var resolvedIssues = await _resolvedIssuesClient.GetIssuesMlCsv(request.WithMock);
            var trainResult = await _machineLearningClient.TrainModel(resolvedIssues, request.ProjectShortName);
            return Ok();
        }

        [HttpPost("SwitchRetrainingState/{projectShortName}")]
        public async Task<IActionResult> SwitchRetrainingState(string projectShortName)
        {
            var result = await _modelRetrainClient.SwitchRetrainForProject(projectShortName);
            return Ok(result);
        }
    }
}