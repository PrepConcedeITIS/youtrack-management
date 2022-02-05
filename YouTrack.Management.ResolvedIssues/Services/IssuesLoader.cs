using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using YouTrack.Management.Shared.Entities;

namespace YouTrack.Management.ResolvedIssues.Services
{
    public class YouTrackIssuesLoader: IIssueLoader
    {
        private readonly HttpClient _httpClient;

        public YouTrackIssuesLoader(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient(Constants.YouTrackHttpClientName);
        }
        
        public async Task<IEnumerable<Issue>> Get()
        {
            var res = await _httpClient.GetAsync("issues?fields=summary,tags(name,$type)");
            var content = await res.Content.ReadAsStringAsync();
            var issues = JsonConvert.DeserializeObject<List<Issue>>(content);
            throw new System.NotImplementedException();
        }
    }

    public interface IIssueLoader
    {
        Task<IEnumerable<Issue>> Get();
    }
}