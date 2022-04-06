using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StackExchange.Redis.Extensions.Core.Abstractions;
using YouTrack.Management.ResolvedIssues.Interfaces;
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
        private readonly HttpClient _mockDataClient;

        public IssuesController(IIssueLoader issueLoader, IRedisClient redisClient, IMapper mapper,
            IHttpClientFactory httpClientFactory)
        {
            _issueLoader = issueLoader;
            _redisClient = redisClient;
            _mapper = mapper;
            _mockDataClient = httpClientFactory.CreateClient("MockDataService");
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

        //todo: rewrite mock part add regenerate param to TrainMockDataGenerationClient.GetMockTrainData
        [HttpGet("machineLearningCsv")]
        public async Task<IActionResult> GetIssuesMlCsv([FromQuery] bool withMock)
        {
            async Task<List<IssueMlCsv>> GetIssuesFromRedis()
            {
                var keys = await _redisClient.GetDefaultDatabase().SearchKeysAsync("*");
                var issues = await _redisClient.GetDefaultDatabase().GetAllAsync<Issue>(keys.ToArray());
                var issueMlCsvs = _mapper.Map<List<IssueMlCsv>>(issues.Values);
                return issueMlCsvs;
            }

            async Task<List<IssueMlCsv>> GetMockData()
            {
                var result = await _mockDataClient.GetStringAsync("MockTrainData");
                return JsonConvert.DeserializeObject<List<IssueMlCsv>>(result);
            }


            var issuesMlTask = GetIssuesFromRedis();
            var tasks = new List<Task>()
            {
                issuesMlTask
            };
            Task<List<IssueMlCsv>> mockTask = null;
            if (withMock)
            {
                mockTask = GetMockData();
            }

            if (withMock)
            {
                tasks.Add(mockTask);
            }

            await Task.WhenAll(tasks);
            var issuesMl = await issuesMlTask;
            if (withMock)
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