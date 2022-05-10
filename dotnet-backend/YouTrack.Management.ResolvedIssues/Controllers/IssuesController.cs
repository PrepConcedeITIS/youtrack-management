using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis.Extensions.Core.Abstractions;
using YouTrack.Management.ResolvedIssues.Interfaces;
using YouTrack.Management.Shared.Entities.Issue;
using YouTrack.Management.TrainMockDataGeneration.Client;

namespace YouTrack.Management.ResolvedIssues.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IssuesController : ControllerBase
    {
        private readonly IIssueLoader _issueLoader;
        private readonly IRedisClient _redisClient;
        private readonly IMapper _mapper;
        private readonly TrainMockDataGenerationClient _trainMockDataGenerationClient;

        public IssuesController(IIssueLoader issueLoader, IRedisClient redisClient, IMapper mapper,
            TrainMockDataGenerationClient trainMockDataGenerationClient)
        {
            _issueLoader = issueLoader;
            _redisClient = redisClient;
            _mapper = mapper;
            _trainMockDataGenerationClient = trainMockDataGenerationClient;
        }

        [HttpGet("resolvedIssuesFromTaskTracker/{projectShortName}")]
        public async Task<IActionResult> GetResolvedIssuesFromTaskTracker(string projectShortName)
        {
            var issues = await _issueLoader.Get(projectShortName);
            return Ok(issues);
        }

        [HttpPost("renewIssuesInStorage/{projectShortName}")]
        public async Task<IActionResult> RenewIssues(string projectShortName)
        {
            var presentedIssues = (await _redisClient.GetDefaultDatabase().SearchKeysAsync($"{projectShortName}-*"))
                .ToHashSet();
            var issues = (await _issueLoader.Get(projectShortName, presentedIssues)).ToList();
            var addingResult = await _redisClient.GetDefaultDatabase()
                .AddAllAsync(issues.Select(x => Tuple.Create(x.IdReadable, x)).ToArray());

            var allIssues = (await _redisClient
                    .GetDefaultDatabase()
                    .GetAllAsync<Issue>(presentedIssues.ToArray())).Values
                .ToList();
            allIssues.AddRange(issues);
            return Ok(allIssues);
        }

        [HttpGet("machineLearningCsv/{projectShortName}")]
        public async Task<IActionResult> GetIssuesMlCsv(string projectShortName, [FromQuery] bool withMock)
        {
            async Task<List<IssueMlCsv>> GetIssuesFromRedis()
            {
                var keys = await _redisClient.GetDefaultDatabase().SearchKeysAsync($"{projectShortName}-*");
                var issues = await _redisClient.GetDefaultDatabase().GetAllAsync<Issue>(keys.ToArray());
                var issueMlCsvs = _mapper.Map<List<IssueMlCsv>>(issues.Values);
                return issueMlCsvs;
            }

            var issuesMlTask = GetIssuesFromRedis();
            var tasks = new List<Task>()
            {
                issuesMlTask
            };
            Task<List<IssueMlCsv>> mockTask = null;
            var addMock = withMock && projectShortName == "AVG";
            if (addMock)
            {
                mockTask = _trainMockDataGenerationClient.GetMockTrainData();
                tasks.Add(mockTask);
            }

            await Task.WhenAll(tasks);
            var issuesMl = await issuesMlTask;
            if (addMock)
            {
                issuesMl.AddRange(await mockTask);
            }

            byte[] bytes;
            using (var memoryStream = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(memoryStream))
                using (var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
                {
                    await csvWriter.WriteRecordsAsync(issuesMl);
                }

                bytes = memoryStream.ToArray();
            }

            return File(bytes, "text/csv");
        }
    }
}