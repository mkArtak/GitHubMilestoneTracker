using AM.Common.Validation;
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

            GitHubClient client = new GitHubClient(new ProductHeaderValue(teamInfo.Organization));
            client.Credentials = new Credentials(accessToken);

            return new GitHubWorkEstimator(client, teamInfo, loggerFactory.CreateLogger<GitHubWorkEstimator>());
        }
    }
}
