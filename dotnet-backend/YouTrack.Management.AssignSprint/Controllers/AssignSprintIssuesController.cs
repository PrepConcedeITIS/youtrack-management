using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using YouTrack.Management.AssigneeActualize.Client;
using YouTrack.Management.AssignSprint.Contracts;
using YouTrack.Management.MachineLearning.Client;
using YouTrack.Management.MachineLearning.Contracts;
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

        public AssignSprintIssuesController(AssigneeActualizeClient assigneeActualizeClient,
            YouTrackClient youTrackClient, MachineLearningClient machineLearningClient)
        {
            _assigneeActualizeClient = assigneeActualizeClient;
            _youTrackClient = youTrackClient;
            _machineLearningClient = machineLearningClient;
        }

        [HttpPost]
        public async Task<IActionResult> Assign(AssignSprintIssuesRequest request)
        {
            var sprintIssues = await _youTrackClient.GetIssuesBySprint(request.SprintName, request.ProjectShortName);
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
            
            //todo: stable matching
            return Ok(predictionResult);
        }
    }
}