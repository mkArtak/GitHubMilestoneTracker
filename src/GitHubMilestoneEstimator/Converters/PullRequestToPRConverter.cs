using MilestoneTracker.Contracts;
using Octokit;

namespace GitHubMilestoneEstimator.Converters
{
    internal class PullRequestToPRConverter
    {
        public PR Convert(PullRequest pr)
        {
            return new PR
            {
                ClosedAt = pr.ClosedAt,
                IssueUrl = pr.IssueUrl,
                Merged = pr.Merged,
                MergedAt = pr.MergedAt,
                Number = pr.Number,
                Title = pr.Title,
                Url = pr.HtmlUrl,
                CreatorLogin = pr.User.Login
            };
        }
    }
}
