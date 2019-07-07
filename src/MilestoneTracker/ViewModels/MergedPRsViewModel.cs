using MilestoneTracker.Contracts;
using System.Collections.Generic;
using System.Linq;

namespace MilestoneTracker.ViewModels
{
    public class MergedPRsViewModel
    {
        private PRListViewModel internalPRs;

        private PRListViewModel communityPRs;

        public IEnumerable<PR> PullRequests { get; set; }

        public PRListViewModel InternalPRs
        {
            get
            {
                if (this.internalPRs == null)
                {
                    this.internalPRs = new PRListViewModel
                    {
                        Title = "Team PRs",
                        PullRequests = this.PullRequests.Where(item => !item.IsExternal),
                        IconRetriever = this.IconRetriever
                    };
                }

                return this.internalPRs;
            }
        }

        public PRListViewModel CommunityPRs
        {
            get
            {
                if (this.communityPRs == null)
                {
                    this.communityPRs = new PRListViewModel
                    {
                        Title = "Community PRs",
                        PullRequests = this.PullRequests.Where(item => item.IsExternal),
                        IconRetriever = this.IconRetriever
                    };
                }

                return this.communityPRs;
            }
        }

        public TeamInfo Team { get; set; }

        public IProfileIconRetriever IconRetriever { get; internal set; }
    }
}
