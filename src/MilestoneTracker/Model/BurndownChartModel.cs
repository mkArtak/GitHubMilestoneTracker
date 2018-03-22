using System.Collections.Generic;

namespace MilestoneTracker.Model
{
    public class BurndownChartModel
    {
        public IEnumerable<string> Milestones { get; set; }

        public string TeamName { get; set; }
    }
}
