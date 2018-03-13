using AM.Common.Validation;
using GitHubMilestoneEstimator.Options;
using Octokit;

namespace GitHub.Client
{
    public class WorkEstimatorFactory
    {
        private readonly GitHubOptions options;
        private readonly IGitHubClient client;

        public WorkEstimatorFactory(GitHubOptions options)
        {
            options.Ensure(nameof(options)).IsNotNull();

            this.options = options;
            this.client = new GitHubClient(new ProductHeaderValue(this.options.Organization));
        }

        public IWorkEstimator Create()
        {
            return new GitHubWorkEstimator(this.client, this.options);
        }
    }
}
