using AM.Common.Validation;
using GitHubMilestoneEstimator.Options;
using Octokit;

namespace GitHub.Client
{
    public class WorkEstimatorFactory
    {
        private readonly GitHubOptions options;
        private readonly GitHubClient client;

        public WorkEstimatorFactory(GitHubOptions options)
        {
            options.Ensure(nameof(options)).IsNotNull();

            this.options = options;
            this.client = new GitHubClient(new ProductHeaderValue(this.options.Organization));
        }

        public IWorkEstimator Create(string accessToken)
        {
            this.client.Credentials = new Credentials(accessToken);

            return new GitHubWorkEstimator(this.client, this.options);
        }
    }
}
