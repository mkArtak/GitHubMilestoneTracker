using AM.Common.Validation;
using Octokit;

namespace GitHub.Client
{
    public class WorkEstimatorFactory
    {
        private readonly IGitHubClient client;

        public WorkEstimatorFactory(string organization)
        {
            organization.Ensure(nameof(organization)).IsNotNullOrWhitespace();

            this.client = new GitHubClient(new ProductHeaderValue(organization));
        }

        public IWorkEstimator Create()
        {
            return new GitHubWorkEstimator(this.client);
        }
    }
}
