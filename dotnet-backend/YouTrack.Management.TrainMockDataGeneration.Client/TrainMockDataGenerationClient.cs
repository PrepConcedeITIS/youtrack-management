using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using YouTrack.Management.Common;
using YouTrack.Management.Shared.Entities.Issue;

namespace YouTrack.Management.TrainMockDataGeneration.Client
{
    public class TrainMockDataGenerationClient : BaseClient
    {
        public TrainMockDataGenerationClient(HttpClient httpClient) : base(httpClient)
        {
        }

        public async Task<List<IssueMlCsv>> GetMockTrainData()
        {
            var url = BuildUrl("MockTrainData");
            var (_, result) = await CallApiGet(url);
            return DeserializeResult<List<IssueMlCsv>>(result);
        }

        public async Task<Stream> GetCsv()
        {
            var url = BuildUrl("MockTrainData/csv");
            var result = await HttpClient.GetStreamAsync(url);
            return result;
        }
    }
}