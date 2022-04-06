using System.Collections.Generic;
using System.Threading.Tasks;
using YouTrack.Management.Shared.Entities.Issue;

namespace YouTrack.Management.ResolvedIssues.Interfaces
{
    public interface IIssueLoader
    {
        Task<IEnumerable<Issue>> Get(HashSet<string> exceptIssuesIdsReadable = null);
    }
}