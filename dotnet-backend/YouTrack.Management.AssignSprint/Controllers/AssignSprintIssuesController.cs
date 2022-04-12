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
                            .ToArray();
                        return ((assignee, index), issuesByResult);
                    })
                    .ToList();
                var issuesMapping = new Dictionary<string, int>();
                for (int j = 0; j < issuesIds.Count; j++)
                {
                    issuesMapping[issuesIds.ElementAt(j)] = j;
                }

                var assigneePreferenceForMatrix =
                    groupedByAssignee.Select(x => x.issuesByResult.Select(y => issuesMapping[y.Id]).ToArray()).ToList();
                assigneePreferenceForMatrix.AddRange(Enumerable.Range(0, assignees.Count)
                    .Select(_ => Enumerable.Range(0, assignees.Count).ToArray()));
                int[][] prefer = assigneePreferenceForMatrix.ToArray();
                new GFG(assignees.Count).StableMarriage(prefer);
            }

            //todo: stable matching
            return Ok(predictionResult);
        }

        // private void Match(List<(string assignee, List<string> issuesByResult)> groupedByAssignee,
        //     List<string> issuesId)
        // {
        //     var predl = issuesId.ToDictionary(x => x, _ => new List<string>());
        //     foreach (var (assignee, issuesByResult) in groupedByAssignee)
        //     {
        //         predl[issuesByResult[0]].Add(assignee);
        //     }
        //
        //     var unMarried
        // }
    }

    public class GFG
    {
        private readonly int _n;

        public GFG(int n)
        {
            _n = n;
        }

        private bool WPrefersM1OverM(int[][] prefer, int w,
            int m, int m1)
        {
            // Check if w prefers m over
            // her current engagement m1
            for (int i = 0; i < _n; i++)
            {
                // If m1 comes before m in list of w,
                // then w prefers her current engagement,
                // don't do anything
                if (prefer[w][i] == m1)
                    return true;

                // If m comes before m1 in w's list,
                // then free her current engagement
                // and engage her with m
                if (prefer[w][i] == m)
                    return false;
            }

            return false;
        }

// Prints stable matching for N boys and
// N girls. Boys are numbered as 0 to
// N-1. Girls are numbered as N to 2N-1.
        public void StableMarriage(int[][] prefer)
        {
            // Stores partner of women. This is our
            // output array that stores passing information.
            // The value of wPartner[i] indicates the partner
            // assigned to woman N+i. Note that the woman
            // numbers between N and 2*N-1. The value -1
            // indicates that (N+i)'th woman is free
            int[] wPartner = new int[_n];

            // An array to store availability of men.
            // If mFree[i] is false, then man 'i' is
            // free, otherwise engaged.
            bool[] mFree = new bool[_n];

            // Initialize all men and women as free
            for (int i = 0; i < _n; i++)
                wPartner[i] = -1;
            int freeCount = _n;

            // While there are free men
            while (freeCount > 0)
            {
                int m;
                for (m = 0; m < _n; m++)
                    if (mFree[m] == false)
                        break;

                for (int i = 0;
                     i < _n &&
                     mFree[m] == false;
                     i++)
                {
                    int w = prefer[m][i];

                    if (wPartner[w - _n] == -1)
                    {
                        wPartner[w - _n] = m;
                        mFree[m] = true;
                        freeCount--;
                    }
                    else
                    {
                        int m1 = wPartner[w - _n];
                        if (WPrefersM1OverM(prefer, w, m, m1) == false)
                        {
                            wPartner[w - _n] = m;
                            mFree[m] = true;
                            mFree[m1] = false;
                        }
                    }
                }
            }

            // Print the solution
            Console.WriteLine("Woman Man");
            for (int i = 0; i < _n; i++)
            {
                Console.Write(" ");
                Console.WriteLine(i + _n + "     " +
                                  wPartner[i]);
            }
        }
    }
}