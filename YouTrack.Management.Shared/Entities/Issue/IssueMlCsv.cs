namespace YouTrack.Management.Shared.Entities.Issue
{
    public class IssueMlCsv
    {
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