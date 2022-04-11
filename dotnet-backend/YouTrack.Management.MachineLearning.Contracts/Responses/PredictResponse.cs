using System.Collections.Generic;

namespace YouTrack.Management.MachineLearning.Contracts.Responses
{
    public class PredictResponse
    {
        public PredictResponse()
        {
        }

        public PredictResponse(List<PredictResponseItem> predictions)
        {
            Predictions = predictions;
        }

        public List<PredictResponseItem> Predictions { get; set; }
    }

    public class PredictResponseItem
    {
        public PredictResponseItem()
        {
        }

        public PredictResponseItem(string id, string assigneeLogin, double grade)
        {
            Id = id;
            Grade = grade;
            AssigneeLogin = assigneeLogin;
        }

        public string Id { get; init; }
        public string AssigneeLogin { get; init; }
        public double Grade { get; init; }
    }
}