using System;
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

        public async Task<List<Issue>> GetResolvedIssuesFromTaskTracker()
        {
            var url = BuildUrl($"{BaseUrl}/resolvedIssuesFromTaskTracker");
            var (_, result) = await CallApiGet(url);
            return DeserializeResult<List<Issue>>(result);
        }

        public async Task<List<Issue>> RenewIssuesInStorage()
        {
            var url = BuildUrl($"{BaseUrl}/renewIssuesInStorage");
            var (_, result) = await CallApiPost(url, null);
            return DeserializeResult<List<Issue>>(result);
        }

        public async Task<Stream> GetIssuesMlCsv(bool withMock)
        {
            var url = BuildUrl($"{BaseUrl}/machineLearningCsv?withMock={withMock}");
            var result = await HttpClient.GetStreamAsync(url);
            return result;
        }
    }
}