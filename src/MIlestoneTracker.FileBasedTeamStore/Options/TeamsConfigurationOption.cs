using MilestoneTracker.Contracts;
using System.Collections.Generic;

namespace MilestoneTracker.DataManagement.Teams.Options
{
    public class TeamsConfigurationOption
    {
        public IEnumerable<TeamInfo> Teams { get; set; }
    }
}
