using System.Collections.Generic;

namespace MilestoneTracker.Contracts
{
    public class TeamInfo
    {
        /// <summary>
        /// Gets or sets the name of the team.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the members of the team.
        /// </summary>
        public IEnumerable<TeamMember> TeamMembers { get; set; }
    }
}
