using AM.Common.Validation;
using GitHubMilestoneEstimator;
using Microsoft.Extensions.Logging;
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
        private readonly IGitHubClient client;
        private readonly TeamInfo options;
        private readonly ILogger logger;

        public GitHubWorkEstimator(IGitHubClient client, TeamInfo options, ILogger<GitHubWorkEstimator> logger)
        {
            this.client = client.Ensure(nameof(client)).IsNotNull().Value;
            this.options = options.Ensure(nameof(options)).IsNotNull().Value;
            this.logger = logger.Ensure(nameof(logger)).IsNotNull().Value;
        }

        public async Task<IEnumerable<WorkItem>> GetAmountOfWorkAsync(IssuesQuery query, CancellationToken cancellationToken)
        {
            query.Ensure(nameof(query)).IsNotNull();

            var searchResults = await QueryIssuesAsync(query, true);
            return searchResults
                .Select(item => new WorkItem
                {
                    Owner = item.Assignee == null ? "Unassigned" : item.Assignee.Login,
                    Cost = this.GetIssueCost(item),
                    Id = item.Number
                }).ToList();
        }

        public async Task<BurndownDTO> GetBurndownDataAsync(IssuesQuery query, CancellationToken cancellationToken)
        {
            IList<Issue> allIssues = await QueryIssuesAsync(query, false);

            double totalAmountOfWork = allIssues.Sum(item => this.GetIssueCost(item));

            DateTimeOffset firstClosedDate = GetDateOfFirstClosedIssue(allIssues, query.Team.FixedIssuesIndicatingLabel);

            DateTimeOffset currentDate = firstClosedDate.Date;
            IList<WorkDTO> result = new List<WorkDTO>();
            double workLeft = totalAmountOfWork;
            int numberOfClosedIssues = 0;
            do
            {
                var issuesClosedOnGivenDate = allIssues
                    .Where(item => item.State.Value == ItemState.Closed && item.ClosedAt.HasValue && item.ClosedAt.Value.Date == currentDate);
                numberOfClosedIssues += issuesClosedOnGivenDate.Count();
                double amountOfWorkClosedOnDate = issuesClosedOnGivenDate.Sum(item => GetIssueCost(item));
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

            return new BurndownDTO { WorkData = result, TotalNumberOfIssues = allIssues.Count, NumberOfIssuesLeft = allIssues.Count - numberOfClosedIssues };
        }

        private async Task<IList<Issue>> QueryIssuesAsync(IssuesQuery query, bool queryForOpenIssuesOnly)
        {
            SearchIssuesRequest request = new SearchIssuesRequest
            {
                Is = queryForOpenIssuesOnly ? new[] { IssueIsQualifier.Issue, IssueIsQualifier.Open } : new[] { IssueIsQualifier.Issue },
                Milestone = query.Milestone,
                Labels = query.FilterLabels
            };

            if (query.Team.LabelsToExclude != null && query.Team.LabelsToExclude.Any())
            {
                request.Exclusions = new SearchIssuesRequestExclusions
                {
                    Labels = query.Team.LabelsToExclude
                };
            }

            request.ApplyRepositoriesFilter(query.Team.Repositories);

            IList<Issue> result = await this.RetrieveAllResultsAsync(request, issue => IssueBelongsToTeam(query.Team, issue));

            /// TODO: Remove this later
            //teamIssues = teamIssues.Where(item => !item.ClosedAt.HasValue || item.ClosedAt.Value >= new DateTimeOffset(2019, 5, 27, 0, 0, 0, TimeSpan.Zero)).ToList();
            return result;
        }

        private bool IssueBelongsToTeam(TeamInfo team, Issue issue)
        {
            if (team.AreaLabels != null && !issue.Labels.Any(lbl => team.AreaLabels.Contains(lbl.Name)))
            {
                return false;
            }

            IEnumerable<string> membersToIncludeInReport = team.GetMembersToIncludeInReport();
            return issue.Assignee == null
                        || membersToIncludeInReport.Any(memberName => String.Equals(memberName, issue.Assignee.Login, StringComparison.OrdinalIgnoreCase));
        }

        private DateTimeOffset GetDateOfFirstClosedIssue(IList<Issue> teamIssues, string fixedIssueIndicatingLabel)
        {
            var closedIssuesQuery = teamIssues
                .Where(item => item.State.Value == ItemState.Closed);
            if (fixedIssueIndicatingLabel != null)
            {
                closedIssuesQuery = closedIssuesQuery.Where(item => item.Labels.Any(l => l.Name == fixedIssueIndicatingLabel));
                this.logger.LogInformation($"Searching for issues with label {fixedIssueIndicatingLabel}");
            }

            DateTimeOffset firstClosedDate = DateTimeOffset.Now.AddDays(-30);

            if (closedIssuesQuery.Any())
            {
                var firstClosedIssue = closedIssuesQuery
                .OrderBy(item => item.ClosedAt)
                .FirstOrDefault();
                if (firstClosedIssue != null)
                {
                    firstClosedDate = firstClosedIssue.ClosedAt.Value;
                    this.logger.LogInformation($"Found first closed issue {firstClosedIssue.Number} which was closed at {firstClosedDate}");
                }
                else
                {
                    this.logger.LogInformation("None of the issues were closed");
                }
            }

            return firstClosedDate;
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
