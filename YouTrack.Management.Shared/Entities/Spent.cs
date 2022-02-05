namespace YouTrack.Management.Shared.Entities
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