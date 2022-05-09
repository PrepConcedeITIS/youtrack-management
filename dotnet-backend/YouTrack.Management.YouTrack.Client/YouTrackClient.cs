using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Force.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using YouTrack.Management.Common;
using YouTrack.Management.Shared.Entities.Activity;
using YouTrack.Management.Shared.Entities.Issue;
using YouTrack.Management.YouTrack.Client.Contracts;

namespace YouTrack.Management.YouTrack.Client
{
    public class YouTrackClient : BaseClient
    {
        private const string IssueFields =
            "fields=id,idReadable,project(name,shortName,id),summary,tags(name),links(linkType(name),issues,direction),customFields(id,name,field(name),value(minutes,login,fullName,name,id))";

        public YouTrackClient(HttpClient httpClient) : base(httpClient)
        {
        }

        protected override T DeserializeResult<T>(string result)
        {
            return JsonConvert.DeserializeObject<T>(result);
        }

        public async Task AssignToIssue(string issueIdReadable, string assigneeLogin)
        {
            var url = BuildUrl($"issues/{issueIdReadable}");
            var body = new
            {
                customFields = new[]
                {
                    new ChangeAssigneeRequest(assigneeLogin)
                }
            };

            var (r, c) = await CallApiPostAsync(url, JsonContent(body));
        }

        public async Task UnAssignIssue(string issueIdReadable)
        {
            var url = BuildUrl($"issues/{issueIdReadable}");
            var body = new
            {
                customFields = new[]
                {
                    new ChangeAssigneeRequest
                    {
                        Value = null
                    }
                }
            };

            var (r, c) = await CallApiPostAsync(url, JsonContent(body));
        }
        
        public async Task<List<Issue>> GetDoneIssues(string projectShortName = "AVG",
            HashSet<string> exceptIssuesIdsReadable = null)
        {
            var query = $"#Feature #Task #Bug #Done #{{Won't fix}} project: {projectShortName}";
            var url = BuildUrl($"issues?{IssueFields}&query={query.PipeTo(HttpUtility.UrlEncode)}");
            var (statusCode, result) = await CallApiGetAsync(url);
            var allResolvedIssues = DeserializeResult<List<Issue>>(result);
            var filtered = allResolvedIssues
                .PipeTo(issues => FilterIssues(issues, exceptIssuesIdsReadable ?? new HashSet<string>(0)));
            var ready = await filtered
                .PipeTo(SetCustomFields)
                .PipeTo(SetStateChangelog);
            return ready;

            IEnumerable<Issue> FilterIssues(IEnumerable<Issue> issues, HashSet<string> exceptIssuesIdsReadable)
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
                            if (estimate is null || spent is null || assignee is null || complexity is null ||
                                successGrade is null || issuesType is null || !issue.Tags.Any())
                                return false;

                            return true;
                        })
                    ;
            }
        }

        public async Task<List<Issue>> GetUnassignedIssuesBySprint(string sprint = "Sprint 1", string projectShortName = "AVG")
        {
            var query = $"Sprint: {"{" + sprint + "}"} #Unresolved #Unassigned #Feature #Task #Bug project: {projectShortName}";
            var url = BuildUrl($"issues?{IssueFields}&query={query.PipeTo(HttpUtility.UrlEncode)}");

            var (statusCode, result) = await CallApiGetAsync(url);
            var rawIssues = DeserializeResult<List<Issue>>(result);

            return rawIssues.PipeTo(FilterIssues).PipeTo(SetCustomFields).ToList();

            IEnumerable<Issue> FilterIssues(IEnumerable<Issue> issues)
            {
                return issues
                    .Where(issue => issue.Tags.Any());
            }
        }
        public async Task<List<Issue>> GetIssuesBySprint(string sprint = "Sprint 1", string projectShortName = "AVG")
        {
            var query = $"Sprint: {"{" + sprint + "}"} #Unresolved #Feature #Task #Bug project: {projectShortName}";
            var url = BuildUrl($"issues?{IssueFields}&query={query.PipeTo(HttpUtility.UrlEncode)}");

            var (statusCode, result) = await CallApiGetAsync(url);
            var rawIssues = DeserializeResult<List<Issue>>(result);

            return rawIssues.PipeTo(FilterIssues).PipeTo(SetCustomFields).ToList();

            IEnumerable<Issue> FilterIssues(IEnumerable<Issue> issues)
            {
                return issues
                    .Where(issue => issue.Tags.Any());
            }
        }

        private IEnumerable<Issue> SetCustomFields(IEnumerable<Issue> issues)
        {
            return issues.Select(issue =>
            {
                var estimate = new Estimate((int?)issue.CustomFields
                    .FirstOrDefault(field => field.Name == "Estimation")
                    ?.Value?.minutes);
                issue.Estimate = estimate.Minutes is null ? null : estimate;

                var spent = new Spent((int?)issue.CustomFields
                    .FirstOrDefault(field => field.Name == "Spent time")
                    ?.Value?.minutes);
                issue.Spent = spent.Minutes is null ? null : spent;

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
            const string activitiesFields =
                @"categories=CustomFieldCategory&fields=activities($type,added($type,name,id),author($typeemail,fullName,id,login,name,ringId),category(id),id,removed($type,name,id),targetMember,timestamp,type,target(id,idReadable))";
            var issuesList = issues.ToList();
            var issuesDict = issuesList.ToDictionary(issue => issue.Id, issue => issue);
            var needToSkip = 0;
            const int take = 50;
            var iterationsCount = (int)Math.Ceiling((decimal)issuesList.Count / take);
            var responses = new List<(HttpStatusCode, string)>();
            for (int i = 0; i < iterationsCount; i++)
            {
                var tasks = issuesList.Skip(needToSkip).Take(take).Select(issue =>
                {
                    var url = BuildUrl($"issues/{issue.Id}/activitiesPage?{activitiesFields}");
                    return CallApiGetAsync(url);
                });
                needToSkip += take;
                responses.AddRange(await Task.WhenAll(tasks));
            }

            var activities = responses
                .Select(response => DeserializeResult<Activities>(response.Item2))
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