using System;
using System.Collections.Generic;
using System.Text;

namespace GitHubMilestoneEstimator.Options
{
    public class GitHubOptions
    {
        public string Organization { get; set; }

        public string[] TeamMembers { get; set; }

        public CostLabelOption[] CostLabels { get; set; }
    }
}
