using System.Net.Http;
using YouTrack.Management.Common;

namespace YouTrack.Management.MachineLearning.Client
{
    public class MachineLearningClient : BaseClient
    {
        public MachineLearningClient(HttpClient httpClient) : base(httpClient)
        {
        }
    }
}