using System.Net.Http;
using System.Threading.Tasks;
using YouTrack.Management.Common;

namespace YouTrack.Management.ModelRetrain.Client
{
    public class ModelRetrainClient: BaseClient
    {
        public ModelRetrainClient(HttpClient httpClient) : base(httpClient)
        {
        }

        public async Task<bool> SwitchRetrainForProject(string projectShortName)
        {
            var url = BuildUrl($"Retrain/ChangeStatus/{projectShortName}");
            var (code, result) = await CallApiPostAsync(url, null);
            return DeserializeResult<bool>(result);
        }
    }
}