using System;

namespace MilestoneTracker.Contracts
{
    public class PR
    {
        public DateTimeOffset? ClosedAt { get; set; }
        public string IssueUrl { get; set; }
        public bool Merged { get; set; }
        public DateTimeOffset? MergedAt { get; set; }
        public int Number { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string CreatorLogin { get; set; }
    }
}
