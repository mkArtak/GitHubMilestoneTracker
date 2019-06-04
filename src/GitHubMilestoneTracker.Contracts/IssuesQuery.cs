using System;
using System.Collections.Generic;
using System.Text;

namespace MilestoneTracker.Contracts
{
    public sealed class IssuesQuery
    {
        public IEnumerable<string> FilterLabels { get; set; }

        public string Milestone { get; set; }

        public TeamInfo Team { get; set; }
    }
}
