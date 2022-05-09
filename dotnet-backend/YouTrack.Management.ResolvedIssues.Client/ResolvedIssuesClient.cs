using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using YouTrack.Management.Common;
using YouTrack.Management.Shared.Entities.Issue;

namespace YouTrack.Management.ResolvedIssues.Client
{
    public class ResolvedIssuesClient : BaseClient
    {
        private const string BaseUrl = "Issues";

        public ResolvedIssuesClient(HttpClient httpClient) : base(httpClient)
        {
        }

        public async Task<List<Issue>> GetResolvedIssuesFromTaskTracker(string projectShortName)
        {
            var url = BuildUrl($"{BaseUrl}/resolvedIssuesFromTaskTracker/{projectShortName}");
            var (_, result) = await CallApiGetAsync(url);
            return DeserializeResult<List<Issue>>(result);
        }

        public async Task<List<Issue>> RenewIssuesInStorage(string projectShortName)
        {
            var url = BuildUrl($"{BaseUrl}/renewIssuesInStorage/{projectShortName}");
            var (_, result) = await CallApiPostAsync(url, null);
            return DeserializeResult<List<Issue>>(result);
        }

        public async Task<Stream> GetIssuesMlCsv(string projectShortName, bool withMock)
        {
            var url = BuildUrl($"{BaseUrl}/machineLearningCsv/{projectShortName}?withMock={withMock}");
            var result = await HttpClient.GetStreamAsync(url);
            return result;
        }
    }
}