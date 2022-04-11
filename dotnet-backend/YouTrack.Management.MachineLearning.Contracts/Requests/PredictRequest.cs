using System.Collections.Generic;

namespace YouTrack.Management.MachineLearning.Contracts.Requests
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

        public PredictRequestItem(string assigneeLogin, string complexity, string[] tags,
            string issueType, string id)
        {
            AssigneeLogin = assigneeLogin;
            Complexity = complexity;
            Tags = tags;
            IssueType = issueType;
            Id = id;
        }

        public string AssigneeLogin { get; init; }
        public string Complexity { get; init; }
        public string[] Tags { get; init; }
        public string IssueType { get; init; }
        public string Id { get; init; }
    }
}