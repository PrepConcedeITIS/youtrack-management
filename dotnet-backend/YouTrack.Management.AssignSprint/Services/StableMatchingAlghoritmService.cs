using System.Collections.Generic;
using System.Linq;
using YouTrack.Management.AssigneeActualize.Contracts;
using YouTrack.Management.AssignSprint.Interfaces;
using YouTrack.Management.MachineLearning.Contracts.Responses;
using YouTrack.Management.Shared.Entities.Issue;

namespace YouTrack.Management.AssignSprint.Services
{
    public class StableMatchingAlghoritmService : IIssueDistributionAlgorithm
    {
        public Dictionary<string, string> Handle(List<AssigneeResponse> assignees, List<Issue> sprintIssues,
            PredictResponse predictionResult)
        {
            var resultDictionary = new Dictionary<string, string>();
            var marryingIterations = sprintIssues.Count / assignees.Count;
            for (int i = 0; i < marryingIterations; i++)
            {
                var issuesIds = sprintIssues
                    .Skip(i * assignees.Count)
                    .Take(assignees.Count)
                    .Select(x => x.IdReadable)
                    .ToHashSet();

                var assigneeMarriage = Marry(issuesIds, assignees, predictionResult);

                foreach (var (assignee, (_, issue)) in assigneeMarriage)
                {
                    resultDictionary.Add(issue, assignee);
                }
            }

            var remainingIssuesIds = sprintIssues
                .Skip(assignees.Count * marryingIterations)
                .Select(x => x.IdReadable)
                .ToHashSet();
            if (remainingIssuesIds.Any())
            {
                var groupedByAssignee = predictionResult.Predictions
                    .GroupBy(predict => predict.AssigneeLogin)
                    .Select((group, index) =>
                    {
                        var assignee = group.Key;
                        var issuesByResult = group
                            .Where(x => remainingIssuesIds.Contains(x.Id))
                            .OrderByDescending(x => x.Grade)
                            .Select(x => x.Id)
                            .ToList();
                        return ((assignee, index), issuesByResult);
                    })
                    .ToDictionary(x => x.Item1.assignee);
                var groupedByIssues = predictionResult.Predictions
                    .Where(x => remainingIssuesIds.Contains(x.Id))
                    .GroupBy(x => x.Id)
                    .Select((group, index) =>
                    {
                        var assigneesByPreference =
                            group.OrderByDescending(x => x.Grade).Select(x => x.AssigneeLogin).ToList();
                        var issueId = group.Key;
                        return ((issueId, index), assigneesByPreference);
                    })
                    .ToDictionary(x => x.Item1.issueId);
                Dictionary<string, (bool married, string assignee)> issueMarriage =
                    remainingIssuesIds.ToDictionary(x => x, _ => (false, default(string)));

                while (issueMarriage.Any(x => !x.Value.married))
                {
                    var currentIssue = issueMarriage.First(x => !x.Value.married);
                    var currentIssueId = currentIssue.Key;
                    var preferredAssigneeByIssue = groupedByIssues[currentIssueId].assigneesByPreference[0];

                    if (issueMarriage.All(x => x.Value.assignee != preferredAssigneeByIssue))
                    {
                        issueMarriage[currentIssueId] = (true, preferredAssigneeByIssue);
                    }
                    else
                    {
                        var previouslySelectedIssue = issueMarriage
                            .Single(x => x.Value.assignee == preferredAssigneeByIssue && x.Value.married).Key;
                        var assigneePreferences = groupedByAssignee[preferredAssigneeByIssue].issuesByResult;
                        var previousIndex = assigneePreferences.IndexOf(previouslySelectedIssue);
                        var currentIndex = assigneePreferences.IndexOf(currentIssueId);
                        if (currentIndex < previousIndex)
                        {
                            issueMarriage[currentIssueId] = (true, preferredAssigneeByIssue);

                            groupedByIssues[previouslySelectedIssue].assigneesByPreference
                                .Remove(preferredAssigneeByIssue);

                            issueMarriage[previouslySelectedIssue] = (false, null);
                        }
                        else
                        {
                            groupedByIssues[currentIssueId].assigneesByPreference
                                .Remove(preferredAssigneeByIssue);
                        }
                    }
                }

                foreach (var (issueId, (_, assignee)) in issueMarriage)
                {
                    resultDictionary.Add(issueId, assignee);
                }
            }

            return resultDictionary;
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
                    var assigneesByPreference =
                        group.OrderByDescending(x => x.Grade).Select(x => x.AssigneeLogin).ToList();
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
                    // индекс предпочтительности - меньше лучше
                    var previousAssigneeIndex = issuePreferences.IndexOf(previouslySelectedAssigneeLogin);
                    var currentAssigneeIndex = issuePreferences.IndexOf(marryingAssigneeLogin);
                    //else if w предпочитает M своему текущему жениху M'
                    if (currentAssigneeIndex < previousAssigneeIndex)
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