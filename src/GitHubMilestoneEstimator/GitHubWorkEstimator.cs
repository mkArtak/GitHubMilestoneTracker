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

        public async Task<double> GetAmountOfWorkAsync(string assignee, string milestone, CancellationToken cancellationToken)
        {
            milestone.Ensure(nameof(milestone)).IsNotNullOrWhitespace();

            SearchIssuesRequest request = new SearchIssuesRequest($"{MilestoneParameterName}:{milestone}")
            {
                Is = new[] { IssueIsQualifier.Open },
            };

            if (assignee != null)
            {
                request.Assignee = assignee;
            }

            SearchIssuesResult result = await this.client.Search.SearchIssues(request);
            return result.Items.Where(item => item.Milestone?.Title == milestone).Sum(issue => this.GetIssueCost(issue));
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

            SearchIssuesResult searchResult = await this.client.Search.SearchIssues(request);
            var teamIssues = searchResult.Items.Where(item => item.Assignee != null && team.TeamMembers.Contains(item.Assignee.Login)).ToList();
            double totalAmountOfWork = teamIssues.Sum(item => this.GetIssueCost(item));
            IDictionary<DateTimeOffset, WorkDTO> daysMap = new Dictionary<DateTimeOffset, WorkDTO>();
            DateTimeOffset firstClosedDate = teamIssues.Select(item => item.ClosedAt).Where(item => item.HasValue).OrderBy(d => d).FirstOrDefault() ?? DateTimeOffset.Now.AddDays(-30);
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

        private double GetWorkDaysForCostLabels(IDictionary<string, int> map, string costLabelName, double daysFactor)
        {
            double result = 0;
            if (map.TryGetValue(costLabelName, out int numberOfItems))
            {
                result = numberOfItems * daysFactor;
            }

            return result;
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
