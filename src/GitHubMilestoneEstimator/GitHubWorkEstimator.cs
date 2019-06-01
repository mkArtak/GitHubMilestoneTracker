using AM.Common.Validation;
using MilestoneTracker.Contracts;
using MilestoneTracker.Contracts.DTO;
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

            SearchIssuesRequest request = new SearchIssuesRequest($"{MilestoneParameterName}:\"{milestone}\"")
            {
                Is = new[] { IssueIsQualifier.Issue, IssueIsQualifier.Open },
            };

            IEnumerable<string> membersToIncludeInReport = GetMembersToIncludeInReport(team);

            IList<Issue> searchResults = await this.RetrieveAllResultsAsync(
                request,
                issue => (issue.Assignee == null || membersToIncludeInReport.Contains(issue.Assignee.Login)));
            string[] teamRepos = team.Repositories ?? new string[] { };
            return searchResults
                .Where(issue => issue.Assignee != null || teamRepos.Any(r => issue.Url.StartsWith($"https://api.github.com/repos/{r}/", StringComparison.OrdinalIgnoreCase)))
                .Select(item => new WorkItem
                {
                    Owner = item.Assignee == null ? "Unassigned" : item.Assignee.Login,
                    Cost = this.GetIssueCost(item)
                }).ToList();
        }

        public async Task<BurndownDTO> GetBurndownDataAsync(TeamInfo team, string milestone, CancellationToken cancellationToken)
        {
            SearchIssuesRequest request = new SearchIssuesRequest($"{MilestoneParameterName}:\"{milestone}\"")
            {
                Is = new[] { IssueIsQualifier.Issue }
            };

            //foreach (var repo in team.Repositories)
            //{
            //    request.Repos.Add(repo);
            //}
            IEnumerable<string> membersToIncludeInReport = GetMembersToIncludeInReport(team);

            IList<Issue> teamIssues = await this.RetrieveAllResultsAsync(
                request,
                issue => issue.Assignee != null
                    && membersToIncludeInReport.Contains(issue.Assignee.Login)
                    && this.GetIssueCost(issue) != 0);

            double totalAmountOfWork = teamIssues.Sum(item => this.GetIssueCost(item));
            DateTimeOffset firstClosedDate = teamIssues
                .Where(item => item.State.Value == ItemState.Closed)
                .Select(item => item.ClosedAt)
                .OrderBy(d => d)
                .FirstOrDefault() ?? DateTimeOffset.Now.AddDays(-30);
            DateTime currentDate = firstClosedDate.Date;

            IList<WorkDTO> result = new List<WorkDTO>();
            double workLeft = totalAmountOfWork;
            int numberOfClosedIssues = 0;
            do
            {
                var closedIssuesQuery = teamIssues
                    .Where(item => item.State.Value == ItemState.Closed && item.ClosedAt.HasValue && item.ClosedAt.Value.Date == currentDate);
                numberOfClosedIssues += closedIssuesQuery.Count();
                double amountOfWorkClosedOnDate = closedIssuesQuery.Sum(item => GetIssueCost(item));
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

            result.Add(new WorkDTO
            {
                Date = currentDate,
                DaysOfWorkLeft = workLeft,
            });

            return new BurndownDTO { WorkData = result, TotalNumberOfIssues = teamIssues.Count, NumberOfIssuesLeft = teamIssues.Count - numberOfClosedIssues };
        }

        private static IEnumerable<string> GetMembersToIncludeInReport(TeamInfo team)
        {
            var result = team.TeamMembers?.Where(item => item.IncludeInReports)?.Select(item => item.Name)?.ToList();
            return result ?? new List<string>();
        }

        private async Task<List<Issue>> RetrieveAllResultsAsync(SearchIssuesRequest request, Func<Issue, bool> filter)
        {
            List<Issue> retrievedIssues = new List<Issue>();
            do
            {
                SearchIssuesResult searchResult;
                IEnumerable<Issue> pageResults;
                int retries = 3;
                do
                {
                    request.PerPage = 100;
                    searchResult = await this.client.Search.SearchIssues(request);
                    pageResults = searchResult.Items.Where(filter);
                    if (retries-- == 0)
                    {
                        throw new TimeoutException("Failed to retrieve all issues in timely manner");
                    }
                } while (searchResult.IncompleteResults);

                retrievedIssues.AddRange(pageResults);
                if (searchResult.Items.Count == 0)
                {
                    break;
                }

                request.Page++;
                if (request.Page == 6)
                {
                    // No need to retrieve more than 500 results
                    break;
                }
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
