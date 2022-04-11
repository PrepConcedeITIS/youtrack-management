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

        public PredictRequestItem(string assigneeLogin, string complexity, string[] tagsConcatenated,
            string issueType, string id)
        {
            AssigneeLogin = assigneeLogin;
            Complexity = complexity;
            TagsConcatenated = tagsConcatenated;
            IssueType = issueType;
            Id = id;
        }

        public string AssigneeLogin { get; init; }
        public string Complexity { get; init; }
        public string[] TagsConcatenated { get; init; }
        public string IssueType { get; init; }
        public string Id { get; init; }
    }
}