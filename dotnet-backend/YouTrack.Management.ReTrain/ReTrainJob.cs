using System;
using System.Threading.Tasks;
using Quartz;
using YouTrack.Management.MachineLearning.Client;
using YouTrack.Management.ResolvedIssues.Client;

namespace YouTrack.Management.ReTrain
{
    public class ReTrainJob: IJob
    {
        private readonly ResolvedIssuesClient _resolvedIssuesClient;
        private readonly MachineLearningClient _machineLearningClient;

        public ReTrainJob(ResolvedIssuesClient resolvedIssuesClient, MachineLearningClient machineLearningClient)
        {
            _resolvedIssuesClient = resolvedIssuesClient;
            _machineLearningClient = machineLearningClient;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var resolvedIssues = await _resolvedIssuesClient.GetIssuesMlCsv(true);
            var trainResult = await _machineLearningClient.TrainModel(resolvedIssues);
        }
    }
}