using GitHub.Client;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DIExtensions
    {
        public static IServiceCollection AddGitHubMilestoneEstimator(this IServiceCollection services)
        {
            services.AddSingleton<WorkEstimatorFactory>();

            return services;
        }
    }
}
