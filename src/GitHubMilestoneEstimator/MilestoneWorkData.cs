using System.Collections.Generic;

namespace GitHub.Client
{
    public class MilestoneWorkData
    {
        public string Milestone { get; set; }

        public IEnumerable<MemberWork> Work { get; set; }
    }
}
