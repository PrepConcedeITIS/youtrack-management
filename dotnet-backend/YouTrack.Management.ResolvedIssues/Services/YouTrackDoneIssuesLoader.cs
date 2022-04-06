using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Force.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using YouTrack.Management.ResolvedIssues.Interfaces;
using YouTrack.Management.Shared.Entities.Activity;
using YouTrack.Management.Shared.Entities.Issue;

namespace YouTrack.Management.ResolvedIssues.Services
{
    public class YouTrackDoneIssuesLoader : IIssueLoader
    {
        private const string IssueFields =
            "fields=id,idReadable,project(name,shortName,id),summary,tags(name),links(linkType(name),issues,direction),customFields(id,name,field(name),value(minutes,login,fullName,name,id))";

        private const string IssueQuery =
            "#Feature #Task #Bug #Done #{Won't fix} project: AVG";

        private const string ActivitiesFields =
            @"categories=CustomFieldCategory&fields=activities($type,added($type,name,id),author($typeemail,fullName,id,login,name,ringId),category(id),id,removed($type,name,id),targetMember,timestamp,type,target(id,idReadable))";

        private readonly HttpClient _httpClient;

        public YouTrackDoneIssuesLoader(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient(Constants.YouTrackHttpClientName);
        }

        public async Task<IEnumerable<Issue>> Get(HashSet<string> exceptIssuesIdsReadable = null)
        {
            var httpResponseMessage = await _httpClient.GetAsync(
                $"issues?{IssueFields}&query={IssueQuery.PipeTo(HttpUtility.UrlEncode)}");
            var content = await httpResponseMessage.Content.ReadAsStringAsync();
            var allResolvedIssues = JsonConvert.DeserializeObject<List<Issue>>(content);
            var filtered = allResolvedIssues
                .PipeTo(issues => FilterIssues(issues, exceptIssuesIdsReadable ?? new HashSet<string>(0)));
            var ready = await filtered
                .PipeTo(SetCustomFields)
                .PipeTo(SetStateChangelog);
            return ready;
        }

        private IEnumerable<Issue> FilterIssues(IEnumerable<Issue> issues, HashSet<string> exceptIssuesIdsReadable)
        {
            return issues
                    .Where(issue => !exceptIssuesIdsReadable.Contains(issue.IdReadable))
                    .Where(issue =>
                    {
                        var subtasks = issue.Links
                            .FirstOrDefault(link =>
                                link.LinkType.Name == "Subtask" &&
                                link.Direction == IssueLink.LinkDirection.OUTWARD)?.Issues;
                        return !subtasks?.Any() ?? true;
                    })
                    .Where(issue =>
                    {
                        var estimate = issue.CustomFields
                            .FirstOrDefault(field => field.Name == "Estimation")
                            ?.Value;
                        var spent = issue.CustomFields
                            .FirstOrDefault(field => field.Name == "Spent time")
                            ?.Value;
                        var assignee = issue.CustomFields
                            .FirstOrDefault(field => field.Name == "Assignee")
                            ?.Value;
                        var complexity = issue.CustomFields
                            .FirstOrDefault(field => field.Name == "Complexity")
                            ?.Value;
                        var successGrade = issue.CustomFields
                            .FirstOrDefault(field => field.Name == "SuccessGrade")
                            ?.Value;
                        var issuesType = issue.CustomFields
                            .FirstOrDefault(field => field.Name == "Type")
                            ?.Value;
                        if (estimate == null || spent == null || assignee == null || complexity == null ||
                            successGrade == null || issuesType == null || !issue.Tags.Any())
                            return false;

                        return true;
                    })
                ;
        }

        private IEnumerable<Issue> SetCustomFields(IEnumerable<Issue> issues)
        {
            return issues.Select(issue =>
            {
                var estimate = new Estimate((int?)issue.CustomFields
                    .FirstOrDefault(field => field.Name == "Estimation")
                    ?.Value.minutes);
                issue.Estimate = estimate;

                var spent = new Spent((int?)issue.CustomFields
                    .FirstOrDefault(field => field.Name == "Spent time")
                    ?.Value.minutes);
                issue.Spent = spent;

                var stateField = issue.CustomFields.FirstOrDefault(field => field.Name == "State")?.Value;
                if (stateField != null)
                {
                    var state = new State((string)stateField.name, (string)stateField.id);
                    issue.State = state;
                }

                var assigneeField = issue.CustomFields.FirstOrDefault(field => field.Name == "Assignee")?.Value;
                if (assigneeField != null)
                {
                    var assignee = new Assignee((string)assigneeField.login,
                        (string)assigneeField.fullName,
                        (string)assigneeField.name,
                        (string)assigneeField.id,
                        (string)assigneeField.email);
                    issue.Assignee = assignee;
                }

                var complexityField = issue.CustomFields.FirstOrDefault(field => field.Name == "Complexity")?.Value;
                if (complexityField != null)
                {
                    var complexity = new Complexity((string)complexityField.id,
                        (string)complexityField.name);
                    issue.Complexity = complexity;
                }

                var successGradeField = issue.CustomFields.FirstOrDefault(field => field.Name == "SuccessGrade")?.Value;
                if (successGradeField != null)
                {
                    var successGrade = new SuccessGrade((string)successGradeField.id,
                        (string)successGradeField.name);
                    issue.SuccessGrade = successGrade;
                }

                var priorityField = issue.CustomFields.FirstOrDefault(field => field.Name == "Priority")?.Value;
                if (priorityField != null)
                {
                    var priority = new Priority((string)priorityField.id,
                        (string)priorityField.name);
                    issue.Priority = priority;
                }

                var issueTypeField = issue.CustomFields.FirstOrDefault(field => field.Name == "Type")?.Value;
                if (issueTypeField != null)
                {
                    var priority = new IssueType((string)issueTypeField.id,
                        (string)issueTypeField.name);
                    issue.IssueType = priority;
                }

                return issue;
            });
        }

        private async Task<List<Issue>> SetStateChangelog(IEnumerable<Issue> issues)
        {
            var issuesList = issues.ToList();
            var issuesDict = issuesList.ToDictionary(issue => issue.Id, issue => issue);
            var needToSkip = 0;
            const int take = 50;
            var iterationsCount = (int)Math.Ceiling((decimal)issuesList.Count / take);
            var responses = new List<HttpResponseMessage>();
            for (int i = 0; i < iterationsCount; i++)
            {
                var tasks = issuesList.Skip(needToSkip).Take(take).Select(issue =>
                {
                    var url = $"issues/{issue.Id}/activitiesPage?{ActivitiesFields}";
                    return _httpClient.GetAsync(url);
                });
                needToSkip += take;
                responses.AddRange(await Task.WhenAll(tasks));
            }

            var activitiesJsonTasks = responses
                .Select(response => response.Content.ReadAsStringAsync()).ToList();
            await Task.WhenAll(activitiesJsonTasks);

            var activities = activitiesJsonTasks
                .Select(jsonTask => jsonTask.Result.PipeTo(JsonConvert.DeserializeObject<Activities>))
                .ToDictionary(list => list.List.FirstOrDefault()?.Target?.Id,
                    list => list.List
                        .Where(activity => activity.TargetMember.Contains("__CUSTOM_FIELD__State"))
                        .Select(activity =>
                        {
                            activity.AddedArr =
                                JsonConvert.DeserializeObject<StateChangedElement[]>(
                                    ((JArray)activity.AddedArrayOrString).ToString());
                            activity.RemovedArr =
                                JsonConvert.DeserializeObject<StateChangedElement[]>(
                                    ((JArray)activity.RemovedArrayOrString).ToString());

                            return activity;
                        })
                        .ToList());

            foreach (var (issueId, activityList) in activities)
            {
                issuesDict[issueId].Changelog = new StateChangelog(activityList.Select(activity =>
                {
                    var toState = activity.AddedArr.FirstOrDefault(x => x.Type == "StateBundleElement")?.Name;
                    var fromState = activity.RemovedArr.FirstOrDefault(x => x.Type == "StateBundleElement")?.Name;
                    var date = DateTimeOffset.FromUnixTimeMilliseconds(activity.Timestamp).DateTime.ToLocalTime();
                    var author = activity.Author;
                    return new StateChangelog.ChangelogItem(fromState, toState, date, author);
                }).ToList());
            }

            return issuesDict.Values.ToList();
        }
    }
}