using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis.Extensions.Core.Abstractions;
using YouTrack.Management.ResolvedIssues.Interfaces;
using YouTrack.Management.Shared.Entities;

namespace YouTrack.Management.ResolvedIssues.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IssuesController : ControllerBase
    {
        private readonly IIssueLoader _issueLoader;
        private readonly IRedisClient _redisClient;

        public IssuesController(IIssueLoader issueLoader, IRedisClient redisClient)
        {
            _issueLoader = issueLoader;
            _redisClient = redisClient;
        }

        [HttpGet("resolvedIssuesFromTaskTracker")]
        public async Task<IActionResult> GetResolvedIssuesFromTaskTracker()
        {
            var issues = await _issueLoader.Get();
            return Ok(issues);
        }

        [HttpPost("renewIssuesInStorage")]
        public async Task<IActionResult> RenewIssues()
        {
            var presentedIssues = (await _redisClient.GetDefaultDatabase().SearchKeysAsync("*")).ToHashSet();
            var issues = (await _issueLoader.Get(presentedIssues)).ToList();
            var addingResult = await _redisClient.GetDefaultDatabase()
                .AddAllAsync(issues.Select(x => Tuple.Create(x.IdReadable, x)).ToArray());

            var allIssues = (await _redisClient
                    .GetDefaultDatabase()
                    .GetAllAsync<Issue>(presentedIssues.ToArray())).Values
                .ToList();
            allIssues.AddRange(issues);
            return Ok(allIssues);
        }
    }
}