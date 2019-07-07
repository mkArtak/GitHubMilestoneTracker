using AM.Common.Validation;
using GitHubMilestoneEstimator;
using Microsoft.Extensions.Logging;
using MilestoneTracker.Contracts;
using Octokit;

namespace GitHub.Client
{
    public class WorkEstimatorFactory
    {
        private readonly ILoggerFactory loggerFactory;

        public WorkEstimatorFactory(ILoggerFactory loggerFactory)
        {
            this.loggerFactory = loggerFactory;
        }

        public IWorkEstimator Create(string accessToken, TeamInfo teamInfo)
        {
            teamInfo.Ensure().IsNotNull();

            GitHubClient client = CreateClient(accessToken, teamInfo);

            return new GitHubWorkEstimator(client, teamInfo, loggerFactory.CreateLogger<GitHubWorkEstimator>());
        }

        public IProfileIconRetriever CreateProfileIconRetriever(string accessToken, TeamInfo teamInfo)
        {
            teamInfo.Ensure().IsNotNull();

            return new GitHubUserProfileIconRetriever(CreateClient(accessToken, teamInfo));
        }

        private static GitHubClient CreateClient(string accessToken, TeamInfo teamInfo)
        {
            GitHubClient client = new GitHubClient(new ProductHeaderValue(teamInfo.Organization));
            client.Credentials = new Credentials(accessToken);
            return client;
        }
    }
}
