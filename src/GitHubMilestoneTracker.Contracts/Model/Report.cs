using System;
using System.Collections.Generic;
using System.Text;

namespace MilestoneTracker.Contracts.Model
{
    public class Report
    {
        /// <summary>
        /// Gets or sets the name of the report.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the list of repositories the team works on.
        /// </summary>
        public IEnumerable<Repository> Repos { get; set; }
    }
}
