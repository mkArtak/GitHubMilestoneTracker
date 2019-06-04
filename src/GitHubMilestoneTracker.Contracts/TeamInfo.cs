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
        /// Gets or sets the organization the teams belongs to.
        /// </summary>
        public string Organization { get; set; }

        /// <summary>
        /// Gets or sets the members of the team.
        /// </summary>
        public TeamMember[] TeamMembers { get; set; }

        /// <summary>
        /// Gets or sets list of labels, used to cost issues for current team.
        /// </summary>
        public CostMarker[] CostLabels { get; set; }

        /// <summary>
        /// Gets or sets the list of area labels the team is handling.
        /// This is useful when the repository is being shared with other teams and there are multiple areas of work.
        /// </summary>
        public string[] AreaLabels { get; set; }

        /// <summary>
        /// Gets or sets the default milestone for the given team to track by default.
        /// </summary>
        public string DefaultMilestonesToTrack { get; set; }

        /// <summary>
        /// Gets or sets the list of repositories the team does work on.
        /// </summary>
        public string[] Repositories { get; set; }

        /// <summary>
        /// Gets or sets the name of the label, which is applied to fixed issues.
        /// </summary>
        public string FixedIssuesIndicatingLabel { get; set; }

        /// <summary>
        /// Gets or sets the list of labels, which indicate the issues to be excluded from query results.
        /// </summary>
        public IEnumerable<string> LabelsToExclude { get; set; }
    }
}
