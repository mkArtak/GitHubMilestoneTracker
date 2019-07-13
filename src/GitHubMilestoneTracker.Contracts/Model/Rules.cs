using System.Collections.Generic;

namespace MilestoneTracker.Contracts.Model
{
    public class Rules
    {
        /// <summary>
        /// Gets or sets the list of area labels the team is handling.
        /// This is useful when the repository is being shared with other teams and there are multiple areas of work.
        /// </summary>
        public IEnumerable<string> LabelsToInclude { get; set; }

        /// <summary>
        /// Gets or sets the list of labels, which indicate the issues to be excluded from query results.
        /// </summary>
        public IEnumerable<string> LabelsToExclude { get; set; }
    }
}
