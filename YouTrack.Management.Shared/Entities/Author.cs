using Newtonsoft.Json;

namespace YouTrack.Management.Shared.Entities
{
    public class Author : Assignee
    {
        public string RingId { get; set; }

        [JsonProperty("$type")]
        public string Type { get; set; }

        public Author(string login, string fullName, string name, string id, string ringId, string email)
            : base(login, fullName, name, id, email)
        {
            RingId = ringId;
        }
    }
}