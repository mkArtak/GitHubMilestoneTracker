using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GitHubMilestoneEstimator
{
    internal static class Extensions
    {
        private static readonly string repoUrlPrefix = "https://api.github.com/repos/";

        public static bool HasLabel(this Issue issue, string label)
        {
            return issue.Labels.Any(l => string.Equals(l.Name, label, StringComparison.InvariantCultureIgnoreCase));
        }

        public static string GetRepository(this Issue issue)
        {
            if (issue.Repository != null)
            {
                return issue.Repository.Name;
            }

            string result = null;
            if (issue.Url.StartsWith(repoUrlPrefix))
            {
                result = issue.Url.Remove(0, repoUrlPrefix.Length);
                result = result.Substring(0, result.IndexOf("/") + 1);
            }

            return result;
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
