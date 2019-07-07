using MilestoneTracker.Contracts;
using System.Collections.Generic;

namespace MilestoneTracker.ViewModels
{
    public class PRListViewModel
    {
        public string Title { get; internal set; }

        public IEnumerable<PR> PullRequests { get; set; }

        public IProfileIconRetriever IconRetriever { get; internal set; }
    }
}
