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
        private const string MilestoneParameterName = "Milestone";

        private readonly IGitHubClient client;
        private readonly TeamInfo options;

        public GitHubWorkEstimator(IGitHubClient client, TeamInfo options)
        {
            this.client = client.Ensure(nameof(client)).IsNotNull().Value;
            this.options = options.Ensure(nameof(options)).IsNotNull().Value;
        }

        public async Task<double> GetAmountOfWorkAsync(string assignee, string milestone, CancellationToken cancellationToken)
        {
            SearchIssuesRequest request = new SearchIssuesRequest()
            {
                Assignee = assignee,
                Parameters = { { MilestoneParameterName, milestone } },
                Is = new[] { IssueIsQualifier.Open }
            };
            SearchIssuesResult result = await this.client.Search.SearchIssues(request);

            IDictionary<string, int> map = new Dictionary<string, int>();
            foreach (var issue in result.Items.Where(item => item.Milestone?.Title == milestone))
            {
                string costLabel = issue.Labels.SingleOrDefault(
                    item => this.options.CostLabels.Any(
                        lbl => lbl.Name.Equals(item.Name, StringComparison.OrdinalIgnoreCase)))?.Name;
                if (costLabel == null)
                {
                    continue;
                }

                if (!map.ContainsKey(costLabel))
                {
                    map[costLabel] = 0;
                }

                map[costLabel]++;
            }

            return this.options.CostLabels.Sum(label => this.GetWorkDaysForCostLabels(map, label.Name, label.Factor));
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
    }
}
