using AM.Common.Validation;
using MilestoneTracker.Contracts;
using Octokit;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GitHubMilestoneEstimator
{
    internal class GitHubUserProfileIconRetriever : IProfileIconRetriever
    {
        private static IDictionary<string, string> profileIconsCache = new ConcurrentDictionary<string, string>();

        private readonly IGitHubClient client;

        public GitHubUserProfileIconRetriever(IGitHubClient client)
        {
            this.client = client.Ensure().IsNotNull().Value;
        }

        public async Task<string> GetUserProfileIconUrl(string login)
        {
            string url;
            if (!profileIconsCache.TryGetValue(login, out url))
            {
                var user = await this.client.User.Get(login);
                url = user.AvatarUrl;
                profileIconsCache.Add(login, url);
            }

            return url;
        }
    }
}
