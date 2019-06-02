using AM.Common.Validation;
using GitHubMilestoneEstimator;
using MilestoneTracker.Contracts;
using MilestoneTracker.Contracts.DTO;
using Octokit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GitHub.Client
{
    internal class GitHubWorkEstimator : IWorkEstimator
    {
        private readonly IGitHubClient client;
        private readonly TeamInfo options;

        public GitHubWorkEstimator(IGitHubClient client, TeamInfo options)
        {
            this.client = client.Ensure(nameof(client)).IsNotNull().Value;
            this.options = options.Ensure(nameof(options)).IsNotNull().Value;
        }

        public async Task<IEnumerable<WorkItem>> GetAmountOfWorkAsync(TeamInfo team, string milestone, IEnumerable<string> labelsFilter, CancellationToken cancellationToken)
        {
            milestone.Ensure(nameof(milestone)).IsNotNullOrWhitespace();

            SearchIssuesRequest request = new SearchIssuesRequest
            {
                Is = new[] { IssueIsQualifier.Issue, IssueIsQualifier.Open },
                Milestone = milestone,
                Labels = labelsFilter
            };
            request.ApplyRepositoriesFilter(team.Repositories);
            IEnumerable<string> membersToIncludeInReport = GetMembersToIncludeInReport(team);
            IList<Issue> searchResults = await this.RetrieveAllResultsAsync(
                request,
                issue => (issue.Assignee == null || membersToIncludeInReport.Contains(issue.Assignee.Login)));
            return searchResults
                .Select(item => new WorkItem
                {
                    Owner = item.Assignee == null ? "Unassigned" : item.Assignee.Login,
                    Cost = this.GetIssueCost(item),
                    Id = item.Number
                }).ToList();
        }

        public async Task<BurndownDTO> GetBurndownDataAsync(TeamInfo team, string milestone, IEnumerable<string> labelsFilter, CancellationToken cancellationToken)
        {
            SearchIssuesRequest request = new SearchIssuesRequest
            {
                Is = new[] { IssueIsQualifier.Issue },
                Milestone = milestone,
                Labels = labelsFilter
            };

            request.ApplyRepositoriesFilter(team.Repositories);

            IEnumerable<string> membersToIncludeInReport = GetMembersToIncludeInReport(team);

            IList<Issue> teamIssues = await this.RetrieveAllResultsAsync(
                request,
                issue => issue.Assignee != null
                    && membersToIncludeInReport.Any(memberName => String.Equals(memberName, issue.Assignee.Login, StringComparison.OrdinalIgnoreCase))
                //&& this.GetIssueCost(issue) != 0
                );

            double totalAmountOfWork = teamIssues.Sum(item => this.GetIssueCost(item));

            DateTimeOffset firstClosedDate = GetDateOfFirstClosedIssue(teamIssues, team.FixedIssuesIndicatingLabel);

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

        private static DateTimeOffset GetDateOfFirstClosedIssue(IList<Issue> teamIssues, string fixedIssueIndicatingLabel)
        {
            var closedIssuesQuery = teamIssues
                .Where(item => item.State.Value == ItemState.Closed);
            if (fixedIssueIndicatingLabel != null)
            {
                closedIssuesQuery = closedIssuesQuery.Where(item => item.Labels.Any(l => l.Name == fixedIssueIndicatingLabel));
            }

            var closedIssues = closedIssuesQuery.Select(item => item.ClosedAt);

            DateTimeOffset firstClosedDate;
            if (closedIssues.Any())
            {
                firstClosedDate = closedIssues
                .Min(item => item ?? DateTimeOffset.UtcNow);
                //.OrderBy(d => d)
                //.FirstOrDefault() ?? DateTimeOffset.Now.AddDays(-30);
            }
            else
            {
                firstClosedDate = DateTimeOffset.UtcNow.AddDays(-30);
            }

            return firstClosedDate;
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
