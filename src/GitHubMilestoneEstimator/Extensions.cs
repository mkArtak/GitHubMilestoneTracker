using Octokit;
using System.Collections.Generic;

namespace GitHubMilestoneEstimator
{
    internal static class Extensions
    {
        private static readonly string repoUrlPrefix = "https://api.github.com/repos/";

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
