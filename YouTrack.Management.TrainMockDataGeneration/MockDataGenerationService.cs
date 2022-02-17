using System;
using System.Collections.Generic;
using System.Linq;
using YouTrack.Management.RelationalDal;
using YouTrack.Management.Shared.Entities.Issue;
using YouTrack.Management.Shared.Enums;

namespace YouTrack.Management.TrainMockDataGeneration
{
    public class MockDataGenerationService
    {
        private static readonly string[] _issueTypes = new[]
        {
            "Bug",
            "Bug",
            "Bug",
            "Task",
            "Task",
            "Feature",
            "Feature",
            "Feature",
            "Feature",
            "Feature",
        };

        private static readonly string[] _complexities = new[]
        {
            CompetenceLevel.Junior.ToString(),
            CompetenceLevel.Junior.ToString(),
            CompetenceLevel.Junior.ToString(),
            CompetenceLevel.Middle.ToString(),
            CompetenceLevel.Middle.ToString(),
            CompetenceLevel.Middle.ToString(),
            CompetenceLevel.Middle.ToString(),
            CompetenceLevel.Middle.ToString(),
            CompetenceLevel.Middle.ToString(),
            CompetenceLevel.Senior.ToString(),
            CompetenceLevel.Senior.ToString(),
        };

        private static readonly string[] _tagsConcatenated = new[]
        {
            string.Join(",", CompetenceType.Dotnet.ToString()),
            string.Join(",", CompetenceType.Dotnet.ToString()),
            string.Join(",", CompetenceType.Dotnet.ToString()),
            string.Join(",", CompetenceType.Dotnet.ToString()),
            string.Join(",", CompetenceType.React.ToString()),
            string.Join(",", CompetenceType.React.ToString()),
            string.Join(",", CompetenceType.React.ToString()),
            string.Join(",", CompetenceType.React.ToString()),
            string.Join(",", CompetenceType.DevOps.ToString()),
            string.Join(",", CompetenceType.DevOps.ToString()),
            string.Join(",", CompetenceType.Dotnet.ToString(), CompetenceType.React.ToString()),
            string.Join(",", CompetenceType.DevOps.ToString(), CompetenceType.Postgres.ToString()),
            string.Join(",", CompetenceType.Dotnet.ToString(), CompetenceType.Postgres.ToString()),
            string.Join(",", CompetenceType.Dotnet.ToString(), CompetenceType.Postgres.ToString()),
        };

        private readonly YoutrackManagementDbContext _dbContext;
        private readonly List<string> _assigneeLogins;
        private readonly Random _random;

        public MockDataGenerationService(YoutrackManagementDbContext dbContext)
        {
            _dbContext = dbContext;
            _assigneeLogins = _dbContext.Assignees.Where(x => !x.Banned).Select(x => x.Login).ToList();
            _random = new Random();
        }

        private IEnumerable<IssueMlCsv> AppendFiveGradeTasks(IEnumerable<IssueMlCsv> issues)
        {
            var ids = Enumerable.Range(issues.Count() + 1, 300).Select(x => $"AVG-0{x}");
            var newIssues = ids.Select(id =>
            {
                var type = _issueTypes[_random.Next(0, _issueTypes.Length)];
                var tags = _tagsConcatenated[_random.Next(0, _tagsConcatenated.Length)];
                var complexity = _complexities[_random.Next(0, _complexities.Length)];
                var assignee = _assigneeLogins[_random.Next(0, _assigneeLogins.Count)];
                var reviewRefuses = _random.Next(0, 1);
                var testRefuses = reviewRefuses > 0 ? 0 : _random.Next(0, 1);
                return new IssueMlCsv(tags, "Average project", assignee, complexity,
                    (_random.Next(-10, 10) / 100).ToString(), 5, type, "sample", reviewRefuses, testRefuses, id);
            });
            return newIssues;
        }
    }
}