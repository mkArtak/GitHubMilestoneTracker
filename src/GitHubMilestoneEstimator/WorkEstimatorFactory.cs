using AM.Common.Validation;
using MilestoneTracker.Contracts;
using Octokit;
using System.Collections.Generic;

namespace GitHub.Client
{
    public class WorkEstimatorFactory
    {
        public WorkEstimatorFactory()
        {
        }

        public IWorkEstimator Create(string accessToken, TeamInfo teamInfo)
        {
            teamInfo.Ensure().IsNotNull();

            GitHubClient client = new GitHubClient(new ProductHeaderValue(teamInfo.Organization));
            client.Credentials = new Credentials(accessToken);

            return new GitHubWorkEstimator(client, teamInfo);
        }
    }
}
