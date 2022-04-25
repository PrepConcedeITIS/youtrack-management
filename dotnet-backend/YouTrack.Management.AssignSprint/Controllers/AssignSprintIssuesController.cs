using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using YouTrack.Management.AssigneeActualize.Client;
using YouTrack.Management.AssignSprint.Contracts;
using YouTrack.Management.AssignSprint.Interfaces;
using YouTrack.Management.MachineLearning.Client;
using YouTrack.Management.MachineLearning.Contracts.Requests;
using YouTrack.Management.YouTrack.Client;

namespace YouTrack.Management.AssignSprint.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AssignSprintIssuesController : ControllerBase
    {
        private readonly YouTrackClient _youTrackClient;
        private readonly AssigneeActualizeClient _assigneeActualizeClient;
        private readonly MachineLearningClient _machineLearningClient;
        private readonly IIssueDistributionAlgorithm _issueDistributionAlgorithm;

        public AssignSprintIssuesController(AssigneeActualizeClient assigneeActualizeClient,
            YouTrackClient youTrackClient, MachineLearningClient machineLearningClient,
            IIssueDistributionAlgorithm issueDistributionAlgorithm)
        {
            _assigneeActualizeClient = assigneeActualizeClient;
            _youTrackClient = youTrackClient;
            _machineLearningClient = machineLearningClient;
            _issueDistributionAlgorithm = issueDistributionAlgorithm;
        }

        [HttpPost]
        public async Task<IActionResult> Assign([FromBody] AssignSprintIssuesRequest request)
        {
            var sprintIssues = await _youTrackClient.GetUnassignedIssuesBySprint(request.SprintName, request.ProjectShortName);
            if (!sprintIssues.Any())
                return Ok();
            var assignees = await _assigneeActualizeClient.GetAssigneesByProject(request.ProjectShortName);
            var predictionRequestItems = new List<PredictRequestItem>();
            foreach (var sprintIssue in sprintIssues)
            {
                foreach (var assignee in assignees)
                {
                    predictionRequestItems.Add(new PredictRequestItem(assignee.Login, sprintIssue.Complexity.Name,
                        sprintIssue.Tags.Select(x => x.Name).ToArray(), sprintIssue.Type, sprintIssue.IdReadable));
                }
            }

            var predictionResult = await _machineLearningClient
                .GetPredictions(new PredictRequest(predictionRequestItems));

            var distributionResult = _issueDistributionAlgorithm.Handle(assignees, sprintIssues, predictionResult);

            var tasks = distributionResult
                .Select(result => _youTrackClient.AssignToIssue(result.Key, result.Value));
            await Task.WhenAll(tasks);

            return Ok();
        }
    }
}