using System.Net.Http;
using YouTrack.Management.Common;

namespace YouTrack.Management.AssignSprint.Client
{
    public class AssignSprintClient : BaseClient
    {
        public AssignSprintClient(HttpClient httpClient) : base(httpClient)
        {
        }
    }
}