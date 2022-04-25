using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using YouTrack.Management.AssigneeActualize.Contracts;
using YouTrack.Management.Common;

namespace YouTrack.Management.AssigneeActualize.Client
{
    public class AssigneeActualizeClient : BaseClient
    {
        public AssigneeActualizeClient(HttpClient httpClient) : base(httpClient)
        {
        }

        public async Task ActualizeAssigneesInDatabase(string projectName)
        {
            var url = BuildUrl($"AssigneeActualize/{projectName}");
            await CallApiPostAsync(url, null);
        }

        public async Task<List<AssigneeResponse>> GetAssigneesByProject(string projectShortName)
        {
            var url = BuildUrl($"AssigneeActualize/{projectShortName}");
            var (_, result) = await CallApiGetAsync(url);
            return DeserializeResult<List<AssigneeResponse>>(result);
        }
    }
}