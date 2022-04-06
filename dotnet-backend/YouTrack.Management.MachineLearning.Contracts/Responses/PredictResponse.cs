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

        public PredictResponseItem(string id, double grade)
        {
            Id = id;
            Grade = grade;
        }

        public string Id { get; init; }
        public double Grade { get; init; }
    }
}