﻿namespace YouTrack.Management.Shared.Entities.Issue
{
    public class State : HasId, ICustomFieldValue
    {
        public string Name { get; set; }

        public State(string id, string name) : base(id)
        {
            Name = name;
        }
    }
}