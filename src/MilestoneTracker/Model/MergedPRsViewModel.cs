using MilestoneTracker.Contracts;
using System.Collections.Generic;
using System.Linq;

namespace MilestoneTracker.Model
{
    public class MergedPRsViewModel
    {
        public IEnumerable<PR> PullRequests { get; set; }

        public IEnumerable<PR> InternalPRs { get => this.PullRequests.Where(item => !item.IsExternal); }

        public IEnumerable<PR> CommunityPRs { get => this.PullRequests.Where(item => item.IsExternal); }

        public TeamInfo Team { get; set; }
        public IProfileIconRetriever IconRetriever { get; internal set; }
    }
}
