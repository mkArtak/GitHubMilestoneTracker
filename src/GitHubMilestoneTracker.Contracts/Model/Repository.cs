using System.Collections.Generic;

namespace MilestoneTracker.Contracts.Model
{
    public class Repository
    {
        /// <summary>
        /// Gets or sets the name of the repository.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the organization the teams belongs to.
        /// </summary>
        public string Organization { get; set; }

        /// <summary>
        /// Gets or sets the rules for the PRs in the repo.
        /// </summary>
        public Rules PRRules { get; set; }

        /// <summary>
        /// Gets or sets the rules for the issues in the repo.
        /// </summary>
        public Rules RepoRules { get; set; }

        /// <summary>
        /// Gets or sets the name of the label, which is applied to fixed issues.
        /// </summary>
        public string FixedIssueLabel { get; set; }

        /// <summary>
        /// Gets or sets list of labels, used to cost issues for current team.
        /// </summary>
        public IEnumerable<CostMarker> CostLabels { get; set; }

        /// <summary>
        /// Gets or sets the milestone for the repository.
        /// </summary>
        public string Milestone { get; set; }
    }
}
