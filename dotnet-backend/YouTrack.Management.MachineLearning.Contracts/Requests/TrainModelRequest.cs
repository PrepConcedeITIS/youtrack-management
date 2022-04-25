namespace YouTrack.Management.MachineLearning.Contracts.Requests
{
    public class TrainModelRequest
    {
        public TrainModelRequest()
        {
            
        }
        public TrainModelRequest(string projectShortName, bool withMock)
        {
            ProjectShortName = projectShortName;
            WithMock = withMock;
        }
        public string ProjectShortName { get; set; }
        public bool WithMock{ get; set; }
    }
}