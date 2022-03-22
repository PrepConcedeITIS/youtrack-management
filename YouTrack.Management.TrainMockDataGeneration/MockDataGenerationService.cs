using System;
using System.Collections.Generic;
using System.Linq;
using Force.Extensions;
using YouTrack.Management.RelationalDal;
using YouTrack.Management.Shared.Entities.Issue;
using YouTrack.Management.Shared.Enums;

namespace YouTrack.Management.TrainMockDataGeneration
{
    public class MockDataGenerationService
    {
        class AssigneeGrade
        {
            public AssigneeGrade(CompetenceType competenceType, CompetenceLevel competenceLevel)
            {
                CompetenceType = competenceType;
                CompetenceLevel = competenceLevel;
            }

            public CompetenceType CompetenceType { get; set; }
            public CompetenceLevel CompetenceLevel { get; set; }
        }

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

        private static readonly Dictionary<string, AssigneeGrade[]> _grades = new()
        {
            {
                "Joe_Cobb",
                new[]
                {
                    new AssigneeGrade(CompetenceType.React, CompetenceLevel.Junior),
                    new AssigneeGrade(CompetenceType.Dotnet, CompetenceLevel.Junior)
                }
            },
            {
                "Dale_Marshall",
                new[]
                {
                    new AssigneeGrade(CompetenceType.DevOps, CompetenceLevel.Junior),
                    new AssigneeGrade(CompetenceType.Dotnet, CompetenceLevel.Middle)
                }
            },
            {
                "Adam_Lucas",
                new[]
                {
                    new AssigneeGrade(CompetenceType.DevOps, CompetenceLevel.Senior),
                    new AssigneeGrade(CompetenceType.React, CompetenceLevel.Middle)
                }
            },
            {
                "Ron_Alexander",
                new[]
                {
                    new AssigneeGrade(CompetenceType.Dotnet, CompetenceLevel.Senior)
                }
            },
            {
                "James_Taylor",
                new[]
                {
                    new AssigneeGrade(CompetenceType.Dotnet, CompetenceLevel.Middle)
                }
            },
            {
                "Mark_Davis",
                new[]
                {
                    new AssigneeGrade(CompetenceType.React, CompetenceLevel.Middle),
                    new AssigneeGrade(CompetenceType.Dotnet, CompetenceLevel.Middle)
                }
            },
            {
                "Rafael_Welch",
                new[]
                {
                    new AssigneeGrade(CompetenceType.React, CompetenceLevel.Senior),
                    new AssigneeGrade(CompetenceType.DevOps, CompetenceLevel.Junior)
                }
            },
        };

        private readonly List<string> _assigneeLogins;
        private readonly Random _random;

        public MockDataGenerationService(YoutrackManagementDbContext dbContext)
        {
            _assigneeLogins = dbContext.Assignees.Where(x => !x.Banned).Select(x => x.Login).ToList();
            _random = new Random();
        }

        public IEnumerable<IssueMlCsv> Handle()
        {
            return Array.Empty<IssueMlCsv>()
                .PipeTo(AppendFiveGradeTasks)
                .PipeTo(AppendFourGradeTasks)
                .PipeTo(AppendThreeGradeTasks)
                .PipeTo(AppendTwoGradeTasks)
                .PipeTo(AppendOneGradeTasks);
        }

        private IEnumerable<IssueMlCsv> AppendFiveGradeTasks(IEnumerable<IssueMlCsv> issues)
        {
            var issueMlCsvs = issues as IssueMlCsv[] ?? issues.ToArray();
            var ids = Enumerable.Range(issueMlCsvs.Length + 1, 200).Select(x => $"AVG-0{x}");
            var newIssues = ids.Select(id =>
            {
                var reviewRefuses = _random.Next(0, 2);
                var testRefuses = reviewRefuses > 0 ? 0 : _random.Next(0, 2);
                var estimationError = _random.Next(-10, 10) / 100.0;
                return GetSample(id, estimationError, reviewRefuses, testRefuses, 5);
            });
            return issueMlCsvs.Concat(newIssues);
        }

        private IEnumerable<IssueMlCsv> AppendFourGradeTasks(IEnumerable<IssueMlCsv> issues)
        {
            var issueMlCsvs = issues as IssueMlCsv[] ?? issues.ToArray();
            var ids = Enumerable.Range(issueMlCsvs.Length + 1, 300).Select(x => $"AVG-0{x}");
            var newIssues = ids.Select(id =>
            {
                var reviewRefuses = _random.Next(0, 3);
                var testRefuses = reviewRefuses >= 1 ? 1 : _random.Next(0, 3);
                var estimationError = _random.Next(-10, 20) / 100.0;
                return GetSample(id, estimationError, reviewRefuses, testRefuses, 4);
            });
            return issueMlCsvs.Concat(newIssues);
        }

        private IEnumerable<IssueMlCsv> AppendThreeGradeTasks(IEnumerable<IssueMlCsv> issues)
        {
            var issueMlCsvs = issues as IssueMlCsv[] ?? issues.ToArray();
            var ids = Enumerable.Range(issueMlCsvs.Length + 1, 100).Select(x => $"AVG-0{x}");
            var newIssues = ids.Select(id =>
            {
                var reviewRefuses = _random.Next(1, 4);
                var testRefuses = reviewRefuses >= 2 ? _random.Next(0, 2) : _random.Next(1, 4);
                return GetSample(id, _random.Next(-20, 30) / 100.0, reviewRefuses, testRefuses, 3);
            });
            return issueMlCsvs.Concat(newIssues);
        }

        private IEnumerable<IssueMlCsv> AppendTwoGradeTasks(IEnumerable<IssueMlCsv> issues)
        {
            var issueMlCsvs = issues as IssueMlCsv[] ?? issues.ToArray();
            var ids = Enumerable.Range(issueMlCsvs.Length + 1, 40).Select(x => $"AVG-0{x}");
            var newIssues = ids.Select(id =>
            {
                var reviewRefuses = _random.Next(2, 5);
                var testRefuses = reviewRefuses >= 3 ? _random.Next(1, 3) : _random.Next(2, 5);
                return GetSample(id, _random.Next(-70, 80) / 100.0, reviewRefuses, testRefuses, 2);
            });
            return issueMlCsvs.Concat(newIssues);
        }

        private IEnumerable<IssueMlCsv> AppendOneGradeTasks(IEnumerable<IssueMlCsv> issues)
        {
            var issueMlCsvs = issues as IssueMlCsv[] ?? issues.ToArray();
            var ids = Enumerable.Range(issueMlCsvs.Length + 1, 20).Select(x => $"AVG-0{x}");
            var newIssues = ids.Select(id =>
            {
                var reviewRefuses = _random.Next(3, 7);
                var testRefuses = _random.Next(3, 7);
                return GetSample(id, _random.Next(-150, 150) / 100.0, reviewRefuses, testRefuses, 1);
            });
            return issueMlCsvs.Concat(newIssues);
        }

        private IssueMlCsv GetSample(string idReadable, double estimationError, int reviewRefuses, int testRefuses,
            int successGrade)
        {
            var type = _issueTypes[_random.Next(0, _issueTypes.Length)];
            var tags = _tagsConcatenated[_random.Next(0, _tagsConcatenated.Length)];
            var complexity = _complexities[_random.Next(0, _complexities.Length)];
            var assignee = _assigneeLogins[_random.Next(0, _assigneeLogins.Count)];
            return new IssueMlCsv(tags, "Average project", assignee, complexity,
                estimationError, successGrade, type, "sample", reviewRefuses, testRefuses, idReadable);
        }
    }
}