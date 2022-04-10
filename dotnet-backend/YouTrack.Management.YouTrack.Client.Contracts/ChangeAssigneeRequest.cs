using System;
using System.Text.Json.Serialization;

namespace YouTrack.Management.YouTrack.Client.Contracts
{
    public class ChangeAssigneeRequest
    {
        public ChangeAssigneeRequest()
        {
        }

        public ChangeAssigneeRequest(string login)
        {
            Value = new AssigneeLogin(login);
        }

        public string Name { get; set; } = "Assignee";

        [JsonPropertyName("$type")]
        public string Type { get; set; } = "SingleUserIssueCustomField";

        public AssigneeLogin Value { get; set; }


        public class AssigneeLogin
        {
            public AssigneeLogin()
            {
            }

            public AssigneeLogin(string login)
            {
                Login = login;
            }

            public string Login { get; set; }
        }
    }
}