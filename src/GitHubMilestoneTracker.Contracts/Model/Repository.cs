using System.Collections.Generic;

namespace MilestoneTracker.Contracts.Model
{
    public class Repository
    {
        public string Name { get; set; }

        public Rules PRRules { get; set; }

        public Rules RepoRules { get; set; }

        /// <summary>
        /// Gets or sets the name of the label, which is applied to fixed issues.
        /// </summary>
        public string FixedIssueLabel { get; set; }

        /// <summary>
        /// Gets or sets list of labels, used to cost issues for current team.
        /// </summary>
        public IEnumerable<CostMarker> CostLabels { get; set; }
    }
}
