using Octokit;

namespace GitHubMilestoneEstimator.Extensions
{
    internal static class GitHubExtensions
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
    }
}
