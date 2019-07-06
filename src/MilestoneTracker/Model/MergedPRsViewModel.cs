using MilestoneTracker.Contracts;
using System.Collections.Generic;

namespace MilestoneTracker.Model
{
    public class MergedPRsViewModel
    {
        public IEnumerable<PR> PullRequests { get; set; }

        public TeamInfo Team { get; set; }
    }
}
