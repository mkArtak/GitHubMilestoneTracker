using System.Collections.Generic;

namespace MilestoneTracker.Contracts
{
    public class MilestoneWorkData
    {
        public string Milestone { get; set; }

        public IEnumerable<MemberWork> Work { get; set; }
    }
}
