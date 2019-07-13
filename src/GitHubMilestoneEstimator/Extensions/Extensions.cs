using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GitHubMilestoneEstimator.Extensions
{
    internal static class Extensions
    {
        public static bool HasLabel(this Issue issue, string label)
        {
            return issue.Labels.Any(l => string.Equals(l.Name, label, StringComparison.InvariantCultureIgnoreCase));
        }

        public static void ApplyLabelsFilter(this SearchIssuesRequest request, IEnumerable<string> labels)
        {
            if (request.Labels == null)
            {
                request.Labels = labels;
            }
            else
            {
                var result = new List<string>();
                result.AddRange(labels);
                result.AddRange(request.Labels);
                request.Labels = result.Distinct(StringComparer.InvariantCultureIgnoreCase);
            }
        }

        public static void ApplyRepositoriesFilter(this SearchIssuesRequest request, IEnumerable<string> repositories)
        {
            if (repositories != null)
            {
                foreach (var repo in repositories)
                {
                    request.Repos.Add(repo);
                }
            }
        }
    }
}
