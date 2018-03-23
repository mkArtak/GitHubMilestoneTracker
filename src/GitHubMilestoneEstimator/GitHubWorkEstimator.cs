using AM.Common.Validation;
using MilestoneTracker.Contracts;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GitHub.Client
{
    internal class GitHubWorkEstimator : IWorkEstimator
    {
        private const string MilestoneParameterName = "milestone";

        private readonly IGitHubClient client;
        private readonly TeamInfo options;

        public GitHubWorkEstimator(IGitHubClient client, TeamInfo options)
        {
            this.client = client.Ensure(nameof(client)).IsNotNull().Value;
            this.options = options.Ensure(nameof(options)).IsNotNull().Value;
        }

        public async Task<IEnumerable<WorkItem>> GetAmountOfWorkAsync(TeamInfo team, string milestone, CancellationToken cancellationToken)
        {
            milestone.Ensure(nameof(milestone)).IsNotNullOrWhitespace();

            SearchIssuesRequest request = new SearchIssuesRequest($"{MilestoneParameterName}:{milestone}")
            {
                Is = new[] { IssueIsQualifier.Issue, IssueIsQualifier.Open },
            };

            IList<Issue> searchResults = await this.RetrieveAllResultsAsync(
                request,
                issue => issue.Assignee != null
                    && team.TeamMembers.Contains(issue.Assignee.Login)
                    && this.GetIssueCost(issue) != 0);
            return searchResults.Select(item => new WorkItem
            {
                Owner = item.Assignee.Login,
                Cost = this.GetIssueCost(item)
            }).ToList();
        }

        public async Task<IEnumerable<WorkDTO>> GetBurndownDataAsync(TeamInfo team, string milestone, CancellationToken cancellationToken)
        {
            SearchIssuesRequest request = new SearchIssuesRequest($"{MilestoneParameterName}:{milestone}")
            {
                Is = new[] { IssueIsQualifier.Issue }
            };

            foreach (var repo in team.Repositories)
            {
                request.Repos.Add(repo);
            }

            IList<Issue> teamIssues = await this.RetrieveAllResultsAsync(
                request,
                issue => issue.Assignee != null
                    && team.TeamMembers.Contains(issue.Assignee.Login)
                    && this.GetIssueCost(issue) != 0);

            double totalAmountOfWork = teamIssues.Sum(item => this.GetIssueCost(item));
            DateTimeOffset firstClosedDate = teamIssues
                .Select(item => item.ClosedAt)
                .Where(item => item.HasValue)
                .OrderBy(d => d)
                .FirstOrDefault() ?? DateTimeOffset.Now.AddDays(-30);
            DateTime currentDate = firstClosedDate.Date;

            IList<WorkDTO> result = new List<WorkDTO>();
            double workLeft = totalAmountOfWork;
            do
            {
                double amountOfWorkClosedOnDate = teamIssues.Where(item => item.ClosedAt.HasValue && item.ClosedAt.Value.Date == currentDate).Sum(item => GetIssueCost(item));
                if (amountOfWorkClosedOnDate > 0)
                {
                    result.Add(new WorkDTO
                    {
                        Date = currentDate,
                        DaysOfWorkLeft = workLeft,
                    });
                }

                workLeft -= amountOfWorkClosedOnDate;
                currentDate = currentDate.AddDays(1);
            } while (currentDate < DateTimeOffset.UtcNow.Date);

            return result;
        }

        private async Task<List<Issue>> RetrieveAllResultsAsync(SearchIssuesRequest request, Func<Issue, bool> filter)
        {
            List<Issue> retrievedIssues = new List<Issue>();
            do
            {
                SearchIssuesResult searchResult = await this.client.Search.SearchIssues(request);
                retrievedIssues.AddRange(searchResult.Items.Where(filter));
                if (!searchResult.IncompleteResults)
                {
                    break;
                }

                request.Page++;
            } while (true);

            return retrievedIssues;
        }

        private double GetIssueCost(Issue issue)
        {
            string costLabel = issue.Labels.SingleOrDefault(
                item => this.options.CostLabels.Any(
                    lbl => lbl.Name.Equals(item.Name, StringComparison.OrdinalIgnoreCase)))?.Name;
            if (costLabel == null)
            {
                return 0;
            }

            CostMarker costMarker = this.options.CostLabels.Where(item => item.Name == costLabel).SingleOrDefault();
            if (costMarker == null)
            {
                return 0;
            }

            return costMarker.Factor;
        }
    }
}
