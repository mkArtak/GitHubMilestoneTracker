using MilestoneTracker.Contracts.Model;
using System.Collections.Generic;
using System.Linq;

namespace MilestoneTracker.Contracts
{
    public class TeamInfo
    {
        private List<string> membersToIncludeInReport;

        /// <summary>
        /// Gets or sets the name of the team.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the organization the teams belongs to.
        /// </summary>
        public string Organization { get; set; }

        /// <summary>
        /// Gets or sets the members of the team.
        /// </summary>
        public IEnumerable<TeamMember> TeamMembers { get; set; }

        /// <summary>
        /// Gets or sets the list of repositories the team works on.
        /// </summary>
        public IEnumerable<Repository> Repos { get; set; }


        /// <summary>
        /// Gets or sets the default milestone for the given team to track by default.
        /// </summary>
        public string DefaultMilestonesToTrack { get; set; }

        public IEnumerable<string> GetMembersToIncludeInReport()
        {
            if (this.membersToIncludeInReport == null)
            {
                this.membersToIncludeInReport = TeamMembers?.Where(item => item.IncludeInReports)?.Select(item => item.Name)?.ToList();
                if (this.membersToIncludeInReport == null)
                    this.membersToIncludeInReport = new List<string>();
            }

            return this.membersToIncludeInReport;
        }
    }
}
