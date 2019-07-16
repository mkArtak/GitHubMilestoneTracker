using AM.Common.Validation;
using GitHubMilestoneEstimator;
using GitHubMilestoneEstimator.Converters;
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
        private readonly TeamInfo teamInfo;
        private readonly ILogger logger;

        private static readonly PullRequestToPRConverter prConverter = new PullRequestToPRConverter();

        public GitHubWorkEstimator(IGitHubClient client, TeamInfo options, ILogger<GitHubWorkEstimator> logger)
        {
            this.client = client.Ensure(nameof(client)).IsNotNull().Value;
            this.teamInfo = options.Ensure(nameof(options)).IsNotNull().Value;
            this.logger = logger.Ensure(nameof(logger)).IsNotNull().Value;
        }

        public async Task<IEnumerable<WorkItem>> GetWorkItemsAsync(IssuesQuery query, CancellationToken cancellationToken)
        {
            query.Ensure(nameof(query)).IsNotNull();

            var searchResults = await QueryIssuesAsync(query);
            return searchResults
                .Select(item => new WorkItem
                {
                    Owner = item.Assignee == null ? "Unassigned" : item.Assignee.Login,
                    Cost = this.GetIssueCost(item),
                    Id = item.Number
                }).ToList();
        }

        public async Task<IEnumerable<PR>> GetPullRequestsAsync(DateTimeOffset mergedOnOrAfter, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            IList<PR> result = new List<PR>();

            var query = new IssuesQuery
            {
                Team = this.teamInfo,
                FilterLabels = null,
                IncludeInvestigations = false,
                Clause = IssuesQueryClause.Merged,
                QueryIssues = false
            };

            var queryResult = await QueryPRsToConsider(query);
            foreach (var issue in queryResult)
            {
                if (issue.PullRequest != null && issue.ClosedAt.HasValue && issue.ClosedAt.Value >= mergedOnOrAfter)
                {
                    result.Add(new PR
                    {
                        ClosedAt = issue.ClosedAt,
                        CreatorLogin = issue.User.Login,
                        Url = issue.HtmlUrl,
                        Merged = issue.ClosedAt.HasValue,
                        Number = issue.Number,
                        MergedAt = issue.ClosedAt.Value,
                        Title = issue.Title,
                    });
                }
            }

            var orgUsers = await this.client.Organization.Member.GetAll(this.teamInfo.Organization);

            foreach (var pr in result)
            {
                pr.IsExternal = !orgUsers.Any(ou => String.Equals(ou.Login, pr.CreatorLogin, StringComparison.InvariantCultureIgnoreCase));
            }

            return result;
        }

        public async Task<BurndownDTO> GetBurndownDataAsync(IssuesQuery query, CancellationToken cancellationToken)
        {
            IList<Issue> allIssues = (await QueryIssuesToConsider(query)).ToList();

            double totalAmountOfWork = allIssues.Sum(item => this.GetIssueCost(item));
            var allClosedIssues = allIssues.Where(item => item.State.Value == ItemState.Closed).ToList();
            DateTimeOffset currentDate = GetDateOfFirstClosedIssue(allClosedIssues).UtcDateTime.Date;
            double workLeft = totalAmountOfWork;
            int numberOfClosedIssues = 0;

            IList<WorkDTO> result = new List<WorkDTO>();
            do
            {
                var issuesClosedOnGivenDate = allClosedIssues.Where(item => item.ClosedAt.Value.UtcDateTime.Date == currentDate);
                if (issuesClosedOnGivenDate.Any())
                {

                    numberOfClosedIssues += issuesClosedOnGivenDate.Count();
                    this.logger.LogInformation($"Found '{issuesClosedOnGivenDate.Count()}' issues closed on '{currentDate.ToString()}'. Total closed issues so far {numberOfClosedIssues}");

                    double amountOfWorkClosedOnDate = issuesClosedOnGivenDate.Sum(item => GetIssueCost(item));
                    if (amountOfWorkClosedOnDate > 0)
                    {
                        result.Add(new WorkDTO
                        {
                            Date = currentDate,
                            DaysOfWorkLeft = workLeft,
                        });
                        workLeft -= amountOfWorkClosedOnDate;
                    }
                }

                currentDate = currentDate.AddDays(1);
            } while (currentDate < DateTimeOffset.UtcNow);

            result.Add(new WorkDTO
            {
                Date = currentDate,
                DaysOfWorkLeft = workLeft,
            });

            return new BurndownDTO
            {
                WorkData = result,
                TotalNumberOfIssues = allIssues.Count,
                NumberOfIssuesLeft = allIssues.Count - numberOfClosedIssues,
            };
        }

        private async Task<IEnumerable<Issue>> QueryIssuesToConsider(IssuesQuery query)
        {
            IEnumerable<Issue> allIssues = await QueryIssuesAsync(query);
            if (!query.IncludeInvestigations)
            {
                allIssues = allIssues.Where(issue =>
                {
                    if (issue.State.Value == ItemState.Closed && !issue.HasLabel(query.Team.FixedIssuesIndicatingLabel))
                    {
                        // Exclude closed issues, which are not marked as fixed.
                        return false;
                    }

                    return true;
                });
            }

            return allIssues;
        }

        private async Task<IList<Issue>> QueryIssuesAsync(IssuesQuery query)
        {
            SearchIssuesRequest request = new SearchIssuesRequest
            {
                Is = GetSearchQueryTerm(query),
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

            IList<Issue> result = await this.QueryResultsAsync(request, issue => IssueBelongsToTeam(query.Team, issue));

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
            return issue.Assignee == null || membersToIncludeInReport.Any(memberName => String.Equals(memberName, issue.Assignee.Login, StringComparison.OrdinalIgnoreCase));
        }

        private DateTimeOffset GetDateOfFirstClosedIssue(IEnumerable<Issue> closedIssuesQuery)
        {
            DateTimeOffset firstClosedDate = DateTimeOffset.UtcNow.AddDays(-30);

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

        private async Task<List<Issue>> QueryResultsAsync(SearchIssuesRequest request, Func<Issue, bool> filter)
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
                item => this.teamInfo.CostLabels.Any(
                    lbl => lbl.Name.Equals(item.Name, StringComparison.OrdinalIgnoreCase)))?.Name;
            if (costLabel == null)
            {
                return 0;
            }

            CostMarker costMarker = this.teamInfo.CostLabels.Where(item => item.Name == costLabel).SingleOrDefault();
            if (costMarker == null)
            {
                return 0;
            }

            return costMarker.Factor;
        }

        private static IEnumerable<IssueIsQualifier> GetSearchQueryTerm(IssuesQuery query)
        {
            List<IssueIsQualifier> terms = new List<IssueIsQualifier>(2);
            terms.Add(query.QueryIssues ? IssueIsQualifier.Issue : IssueIsQualifier.PullRequest);
            switch (query.Clause)
            {
                case IssuesQueryClause.All:
                    break;

                case IssuesQueryClause.Open:
                    terms.Add(IssueIsQualifier.Open);
                    break;

                case IssuesQueryClause.Closed:
                    terms.Add(IssueIsQualifier.Closed);
                    break;

                case IssuesQueryClause.Merged:
                    terms.Add(IssueIsQualifier.Merged);
                    break;

                default:
                    throw new InvalidOperationException("Unkonwn query type");
            }

            return terms;
        }

        private async Task<IEnumerable<Issue>> QueryPRsToConsider(IssuesQuery query)
        {
            SearchIssuesRequest request = new SearchIssuesRequest
            {
                Is = GetSearchQueryTerm(query),
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

            IList<Issue> result = await this.QueryResultsAsync(request, issue => IssueBelongsToTeam(query.Team, issue));

            /// TODO: Remove this later
            //teamIssues = teamIssues.Where(item => !item.ClosedAt.HasValue || item.ClosedAt.Value >= new DateTimeOffset(2019, 5, 27, 0, 0, 0, TimeSpan.Zero)).ToList();
            return result;
        }
    }
}
