using MilestoneTracker.Contracts;
using System.Collections.Generic;

namespace MilestoneTracker.DataManagement.Teams.Options
{
    internal class TeamsConfigurationOption
    {
        public IEnumerable<TeamInfo> Teams { get; set; }
    }
}
