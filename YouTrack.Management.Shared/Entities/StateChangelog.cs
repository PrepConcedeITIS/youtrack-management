using System;
using System.Collections.Generic;

namespace YouTrack.Management.Shared.Entities
{
    public class StateChangelog
    {
        public List<ChangelogItem> History { get; set; }

        public StateChangelog(List<ChangelogItem> history)
        {
            History = history;
        }

        public class ChangelogItem
        {
            public ChangelogItem(string fromState, string toState, DateTime dateTime, Author author)
            {
                FromState = fromState;
                ToState = toState;
                DateTime = dateTime;
                Author = author;
            }

            public string FromState { get; set; }
            public string ToState { get; set; }
            public DateTime DateTime { get; set; }
            public Author Author { get; set; }
        }
    }
}