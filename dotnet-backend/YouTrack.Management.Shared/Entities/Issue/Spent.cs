namespace YouTrack.Management.Shared.Entities.Issue
{
    public class Spent : ICustomFieldValue
    {
        public int? Minutes { get; set; }

        public Spent(int? minutes)
        {
            Minutes = minutes;
        }
    }
}