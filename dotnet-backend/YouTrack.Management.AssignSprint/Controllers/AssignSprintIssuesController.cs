using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using YouTrack.Management.AssigneeActualize.Client;
using YouTrack.Management.AssigneeActualize.Contracts;
using YouTrack.Management.AssignSprint.Contracts;
using YouTrack.Management.MachineLearning.Client;
using YouTrack.Management.MachineLearning.Contracts.Requests;
using YouTrack.Management.MachineLearning.Contracts.Responses;
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

            var resultDictionary = new Dictionary<string, string>();
            for (int i = 0; i < sprintIssues.Count / assignees.Count; i++)
            {
                var issuesIds = sprintIssues
                    .Skip(i * assignees.Count)
                    .Take(assignees.Count)
                    .Select(x => x.IdReadable)
                    .ToHashSet();
                var groupedByAssignee = predictionResult.Predictions
                    .GroupBy(predict => predict.AssigneeLogin)
                    .Select((group, index) =>
                    {
                        var assignee = group.Key;
                        var issuesByResult = group
                            .Where(x => issuesIds.Contains(x.Id))
                            .OrderByDescending(x => x.Grade)
                            .Select(x => x.Id)
                            .ToList();
                        return ((assignee, index), issuesByResult);
                    })
                    .ToDictionary(x => x.Item1.assignee);
                var groupedByIssues = predictionResult.Predictions
                    .Where(x => issuesIds.Contains(x.Id))
                    .GroupBy(x => x.Id)
                    .Select((group, index) =>
                    {
                        var assigneesByPreference = group.OrderBy(x => x.Grade).Select(x => x.AssigneeLogin).ToList();
                        var issueId = group.Key;
                        return ((issueId, index), assigneesByPreference);
                    })
                    .ToDictionary(x => x.Item1.issueId);

                Dictionary<string, (bool married, string issue)> assigneeMarriage =
                    assignees.ToDictionary(x => x.Login, _ => (false, default(string)));

                while (assigneeMarriage.Any(x => !x.Value.married))
                {
                    var freeAssignee = assigneeMarriage.First(x => !x.Value.married);
                    var marryingAssigneeLogin = freeAssignee.Key;
                    var preferredIssueByAssignee = groupedByAssignee[marryingAssigneeLogin].issuesByResult[0];

                    //задача свободна
                    if (assigneeMarriage.All(x => x.Value.issue != preferredIssueByAssignee))
                    {
                        assigneeMarriage[marryingAssigneeLogin] = (true, preferredIssueByAssignee);
                    }
                    else
                    {
                        var previouslySelectedAssigneeLogin = assigneeMarriage
                            .Single(x => x.Value.issue == preferredIssueByAssignee && x.Value.married).Key;

                        var issuePreferences = groupedByIssues[preferredIssueByAssignee].assigneesByPreference;
                        // индекс предпочтительности - больше лучше
                        var previousAssigneeIndex = issuePreferences.IndexOf(previouslySelectedAssigneeLogin);
                        var currentAssigneeIndex = issuePreferences.IndexOf(marryingAssigneeLogin);
                        //else if w предпочитает M своему текущему жениху M'
                        if (currentAssigneeIndex > previousAssigneeIndex)
                        {
                            assigneeMarriage[marryingAssigneeLogin] = (true, preferredIssueByAssignee);

                            //вычёркиваем w из списка предпочтений M'
                            groupedByAssignee[previouslySelectedAssigneeLogin].issuesByResult
                                .Remove(preferredIssueByAssignee);

                            //помечаем M' свободным
                            assigneeMarriage[previouslySelectedAssigneeLogin] = (false, null);
                        }
                        else
                        {
                            //вычёркиваем w из списка предпочтений M
                            groupedByAssignee[marryingAssigneeLogin].issuesByResult.Remove(preferredIssueByAssignee);
                        }
                    }
                }

                foreach (var (assignee, (_, issue)) in assigneeMarriage)
                {
                    resultDictionary.Add(issue, assignee);
                }
            }

            //todo: stable matching
            return Ok(predictionResult);
        }

        private Dictionary<string, (bool married, string issue)> Marry(HashSet<string> issuesIds,
            List<AssigneeResponse> assignees, PredictResponse predictionResult)
        {
            var groupedByAssignee = predictionResult.Predictions
                .GroupBy(predict => predict.AssigneeLogin)
                .Select((group, index) =>
                {
                    var assignee = group.Key;
                    var issuesByResult = group
                        .Where(x => issuesIds.Contains(x.Id))
                        .OrderByDescending(x => x.Grade)
                        .Select(x => x.Id)
                        .ToList();
                    return ((assignee, index), issuesByResult);
                })
                .ToDictionary(x => x.Item1.assignee);
            var groupedByIssues = predictionResult.Predictions
                .Where(x => issuesIds.Contains(x.Id))
                .GroupBy(x => x.Id)
                .Select((group, index) =>
                {
                    var assigneesByPreference = group.OrderBy(x => x.Grade).Select(x => x.AssigneeLogin).ToList();
                    var issueId = group.Key;
                    return ((issueId, index), assigneesByPreference);
                })
                .ToDictionary(x => x.Item1.issueId);

            Dictionary<string, (bool married, string issue)> assigneeMarriage =
                assignees.ToDictionary(x => x.Login, _ => (false, default(string)));

            while (assigneeMarriage.Any(x => !x.Value.married))
            {
                var freeAssignee = assigneeMarriage.First(x => !x.Value.married);
                var marryingAssigneeLogin = freeAssignee.Key;
                var preferredIssueByAssignee = groupedByAssignee[marryingAssigneeLogin].issuesByResult[0];

                //задача свободна
                if (assigneeMarriage.All(x => x.Value.issue != preferredIssueByAssignee))
                {
                    assigneeMarriage[marryingAssigneeLogin] = (true, preferredIssueByAssignee);
                }
                else
                {
                    var previouslySelectedAssigneeLogin = assigneeMarriage
                        .Single(x => x.Value.issue == preferredIssueByAssignee && x.Value.married).Key;

                    var issuePreferences = groupedByIssues[preferredIssueByAssignee].assigneesByPreference;
                    // индекс предпочтительности - больше лучше
                    var previousAssigneeIndex = issuePreferences.IndexOf(previouslySelectedAssigneeLogin);
                    var currentAssigneeIndex = issuePreferences.IndexOf(marryingAssigneeLogin);
                    //else if w предпочитает M своему текущему жениху M'
                    if (currentAssigneeIndex > previousAssigneeIndex)
                    {
                        assigneeMarriage[marryingAssigneeLogin] = (true, preferredIssueByAssignee);

                        //вычёркиваем w из списка предпочтений M'
                        groupedByAssignee[previouslySelectedAssigneeLogin].issuesByResult
                            .Remove(preferredIssueByAssignee);

                        //помечаем M' свободным
                        assigneeMarriage[previouslySelectedAssigneeLogin] = (false, null);
                    }
                    else
                    {
                        //вычёркиваем w из списка предпочтений M
                        groupedByAssignee[marryingAssigneeLogin].issuesByResult.Remove(preferredIssueByAssignee);
                    }
                }
            }

            return assigneeMarriage;
        }
    }
}