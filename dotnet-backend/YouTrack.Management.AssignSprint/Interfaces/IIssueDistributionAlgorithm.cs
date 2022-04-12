using System.Collections.Generic;
using YouTrack.Management.AssigneeActualize.Contracts;
using YouTrack.Management.MachineLearning.Contracts.Responses;
using YouTrack.Management.Shared.Entities.Issue;

namespace YouTrack.Management.AssignSprint.Interfaces
{
    public interface IIssueDistributionAlgorithm
    {
        Dictionary<string, string> Handle(List<AssigneeResponse> assignees, List<Issue> sprintIssues,
            PredictResponse predictionResult);
    }
}