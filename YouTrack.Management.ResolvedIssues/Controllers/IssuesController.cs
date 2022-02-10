using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CsvHelper;
using Force.Extensions;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis.Extensions.Core.Abstractions;
using YouTrack.Management.ResolvedIssues.Interfaces;
using YouTrack.Management.Shared.Entities;
using YouTrack.Management.Shared.Entities.Issue;

namespace YouTrack.Management.ResolvedIssues.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IssuesController : ControllerBase
    {
        private readonly IIssueLoader _issueLoader;
        private readonly IRedisClient _redisClient;
        private readonly IMapper _mapper;

        public IssuesController(IIssueLoader issueLoader, IRedisClient redisClient, IMapper mapper)
        {
            _issueLoader = issueLoader;
            _redisClient = redisClient;
            _mapper = mapper;
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

        [HttpGet("machineLearningCsv")]
        public async Task<IActionResult> GetIssuesMlCsv()
        {
            var keys = await _redisClient.GetDefaultDatabase().SearchKeysAsync("*");
            var issues = await _redisClient.GetDefaultDatabase().GetAllAsync<Issue>(keys.ToArray());
            var issuesMl = _mapper.Map<ICollection<IssueMlCsv>>(issues.Values);

            StreamWriter streamWriter = null;
            CsvWriter csvWriter = null;
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    streamWriter = new StreamWriter(memoryStream);
                    csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture);
                    csvWriter.WriteHeader<IssueMlCsv>();
                    csvWriter.NextRecord();
                    foreach (var record in issuesMl)
                    {
                        csvWriter.WriteRecord(record);
                        csvWriter.NextRecord();
                    }

                    return File(memoryStream, "text/csv");
                }
            }
            finally
            {
                if (streamWriter != null) streamWriter.Dispose();
                if (csvWriter != null) csvWriter.Dispose();
            }
        }
    }
}