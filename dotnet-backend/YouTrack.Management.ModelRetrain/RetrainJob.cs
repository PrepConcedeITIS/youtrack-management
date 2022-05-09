using System.Linq;
using System.Threading.Tasks;
using Quartz;
using YouTrack.Management.MachineLearning.Client;
using YouTrack.Management.ModelRetrain.EF;
using YouTrack.Management.ResolvedIssues.Client;

namespace YouTrack.Management.ModelRetrain
{
    public class RetrainJob : IJob
    {
        private readonly ResolvedIssuesClient _resolvedIssuesClient;
        private readonly MachineLearningClient _machineLearningClient;
        private readonly RetrainDbContext _dbContext;

        public RetrainJob(ResolvedIssuesClient resolvedIssuesClient, MachineLearningClient machineLearningClient,
            RetrainDbContext dbContext)
        {
            _resolvedIssuesClient = resolvedIssuesClient;
            _machineLearningClient = machineLearningClient;
            _dbContext = dbContext;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var projectsToRetrain = _dbContext.Projects.Where(x => x.RetrainEnabled).ToList();
            var tasks = projectsToRetrain.Select(async x =>
            {
                var resolvedIssues = await _resolvedIssuesClient.GetIssuesMlCsv(x.ProjectKey, true);
                var trainResult = await _machineLearningClient.TrainModel(resolvedIssues, x.ProjectKey);
            });
            await Task.WhenAll(tasks);
        }
    }
}