using System.Collections.Generic;

namespace MilestoneTracker.Contracts
{
    public sealed class IssuesQuery
    {
        public IEnumerable<string> FilterLabels { get; set; }

        public string Milestone { get; set; }

        public TeamInfo Team { get; set; }

        /// <summary>
        /// Gets or sets a boolean value indicating whether the query result should include work on investigations and engagement with community, which resulted in no actual code changes.
        /// </summary>
        public bool IncludeInvestigations { get; set; }

        /// <summary>
        /// true, to query for Issues.
        /// false, to query for PRs.
        /// </summary>
        public bool QueryIssues { get; set; }

        public IssuesQueryClause Clause { get; set; }
    }
}
