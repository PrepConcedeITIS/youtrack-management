using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace YouTrack.Management.Shared.Entities.Activity
{
    public class Activities
    {
        [JsonProperty("activities")]
        public List<Activity> List { get; set; }
    }
    public class Activity : HasId
    {
        public DateTime DateTime => DateTimeOffset.FromUnixTimeMilliseconds(Timestamp).DateTime.ToLocalTime();
        public StateChangedElement[] AddedArr { get; set; }
        public StateChangedElement[] RemovedArr { get; set; }
        public string TargetMember { get; set; }
        public Author Author { get; set; }
        public Category Category { get; set; }

        [JsonProperty("timestamp")]
        public long Timestamp { get; set; }

        [JsonProperty("$type")]
        public string Type { get; set; }
        
        [JsonProperty("added")]
        public dynamic AddedArrayOrString { get; set; }
        [JsonProperty("removed")]
        public dynamic RemovedArrayOrString { get; set; }

        public ActivityTarget Target { get; set; }
        public Activity(string id, StateChangedElement[] addedArr, StateChangedElement[] removedArr, string targetMember,
            Author author, Category category, long timestamp, string type) : base(id)
        {
            AddedArr = addedArr;
            RemovedArr = removedArr;
            TargetMember = targetMember;
            Author = author;
            Category = category;
            Timestamp = timestamp;
            Type = type;
        }
    }

    public class ActivityTarget : HasId
    {
        public string IdReadable { get; set; }

        [JsonProperty("$type")]
        public string Type { get; set; }

        public ActivityTarget(string id) : base(id)
        {
        }
    }
}