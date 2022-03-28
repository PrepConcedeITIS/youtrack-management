namespace YouTrack.Management.Shared.Entities.Issue
{
    public class Estimate : ICustomFieldValue
    {
        public Estimate(int? minutes)
        {
            Minutes = minutes;
        }

        public int? Minutes { get; set; }
    }
}