using System.Net.Http;
using System.Threading.Tasks;
using YouTrack.Management.AssignSprint.Contracts;
using YouTrack.Management.Common;

namespace YouTrack.Management.AssignSprint.Client
{
    public class AssignSprintClient : BaseClient
    {
        public AssignSprintClient(HttpClient httpClient) : base(httpClient)
        {
        }

        public async Task AssignIssuesToSprint(AssignSprintIssuesRequest request)
        {
            var url = BuildUrl("AssignSprintIssues");
            var (_, result) = await CallApiPostAsync(url, JsonContent(request));
        }
    }
}