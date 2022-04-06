namespace YouTrack.Management.MachineLearning.Contracts.Responses
{
    public class TrainResponse
    {
        public TrainResponse()
        {
        }

        public TrainResponse(double accuracy)
        {
            Accuracy = accuracy;
        }

        public double Accuracy { get; set; }
    }
}