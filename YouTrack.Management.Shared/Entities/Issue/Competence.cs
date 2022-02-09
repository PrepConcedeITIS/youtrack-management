using YouTrack.Management.Shared.Enums;

namespace YouTrack.Management.Shared.Entities.Issue
{
    public class Competence : HasId<long>
    {
        public Competence(long id, CompetenceType competenceType, CompetenceLevel competenceValue) : base(id)
        {
            CompetenceType = competenceType;
            CompetenceValue = competenceValue;
        }

        protected Competence()
        {
        }

        public CompetenceType CompetenceType { get; set; }
        public CompetenceLevel CompetenceValue { get; set; }
    }
}