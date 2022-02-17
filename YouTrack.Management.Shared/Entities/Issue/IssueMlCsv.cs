namespace YouTrack.Management.Shared.Entities.Issue
{
    public class IssueMlCsv
    {
        public IssueMlCsv()
        {
        }

        public IssueMlCsv(string tagsConcatenated, string projectName, string assigneeLogin, string complexity,
            string estimationError, int successGrade, string issueType, string summary, int reviewRefuses,
            int testRefuses, string idReadable)
        {
            TagsConcatenated = tagsConcatenated;
            ProjectName = projectName;
            AssigneeLogin = assigneeLogin;
            Complexity = complexity;
            EstimationError = estimationError;
            SuccessGrade = successGrade;
            IssueType = issueType;
            Summary = summary;
            ReviewRefuses = reviewRefuses;
            TestRefuses = testRefuses;
            IdReadable = idReadable;
        }
        public string TagsConcatenated { get; set; }
        public string ProjectName { get; set; }
        public string AssigneeLogin { get; set; }
        public string Complexity { get; set; }
        public string EstimationError { get; set; }
        public int SuccessGrade { get; set; }
        public string IssueType { get; set; }


        // Пока под вопросом
        public string Summary { get; set; }
        public int ReviewRefuses { get; set; }
        public int TestRefuses { get; set; }

        /// <summary>
        /// Техническое поле, не учавствует в обучении модели
        /// </summary>
        public string IdReadable { get; set; }
    }
}