using System.Collections.Generic;
using YouTrack.Management.Shared.Enums;

namespace YouTrack.Management.MachineLearning.Contracts
{
    public class PredictRequest
    {
        public PredictRequest()
        {
        }

        public PredictRequest(List<PredictRequestItem> data)
        {
            Data = data;
        }

        public List<PredictRequestItem> Data { get; set; }
    }

    public class PredictRequestItem
    {
        public PredictRequestItem()
        {
        }

        public PredictRequestItem(string assigneeLogin, CompetenceLevel complexity, CompetenceType[] tags,
            string issueType, string id)
        {
            AssigneeLogin = assigneeLogin;
            Complexity = complexity;
            Tags = tags;
            IssueType = issueType;
            Id = id;
        }

        public string AssigneeLogin { get; init; }
        public CompetenceLevel Complexity { get; init; }
        public CompetenceType[] Tags { get; init; }
        public string IssueType { get; init; }
        public string Id { get; init; }
    }
}