using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using YouTrack.Management.Common;
using YouTrack.Management.MachineLearning.Contracts;
using YouTrack.Management.MachineLearning.Contracts.Requests;
using YouTrack.Management.MachineLearning.Contracts.Responses;

namespace YouTrack.Management.MachineLearning.Client
{
    public class MachineLearningClient : BaseClient
    {
        public MachineLearningClient(HttpClient httpClient) : base(httpClient)
        {
        }

        public async Task<PredictResponse> GetPredictions(PredictRequest request, string projectShortName)
        {
            var url = BuildUrl($"predict/{projectShortName}");
            var (_, result) = await CallApiPostAsync(url, JsonContent(request));
            return DeserializeResult<PredictResponse>(result);
        }

        public async Task<TrainResponse> TrainModel(Stream csvStream, string projectShortName)
        {
            var url = BuildUrl($"train/{projectShortName}");
            var multipartFormDataContent = new MultipartFormDataContent();
            var csvContent = new StreamContent(csvStream);
            csvContent.Headers.ContentType = new MediaTypeHeaderValue("text/csv");
            multipartFormDataContent.Add(csvContent, "train-data", "train-data");
            var (statusCode, result) = await CallApiAsync(client => client.PostAsync(url, multipartFormDataContent));
            return DeserializeResult<TrainResponse>(result);
        }
    }
}