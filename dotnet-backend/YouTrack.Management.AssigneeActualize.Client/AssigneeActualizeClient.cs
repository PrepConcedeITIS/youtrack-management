using System.Net.Http;
using System.Threading.Tasks;
using YouTrack.Management.Common;

namespace YouTrack.Management.AssigneeActualize.Client
{
    public class AssigneeActualizeClient : BaseClient
    {
        public AssigneeActualizeClient(HttpClient httpClient) : base(httpClient)
        {
        }

        public async Task ActualizeAssigneesInDatabase()
        {
            var url = BuildUrl("AssigneeActualize");
            await CallApiGet(url);
        }
    }
}