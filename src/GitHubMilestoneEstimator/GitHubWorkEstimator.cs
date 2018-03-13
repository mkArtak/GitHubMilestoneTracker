using AM.Common.Validation;
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

        public GitHubWorkEstimator(IGitHubClient client)
        {
            this.client = client.Ensure(nameof(client)).IsNotNull().Value;
        }

        public async Task<int> GetAmountOfWorkAsync(string assignee, string milestone, CancellationToken cancellationToken)
        {
            SearchIssuesRequest request = new SearchIssuesRequest()
            {
                Assignee = assignee,
                Parameters = { { "Milestone", milestone } },
                Is = new[] { IssueIsQualifier.Open }
            };
            SearchIssuesResult result = await this.client.Search.SearchIssues(request);

            IDictionary<string, int> map = new Dictionary<string, int>();
            foreach (var issue in result.Items.Where(item => item.Milestone?.Title == milestone))
            {
                string costLabel = issue.Labels.SingleOrDefault(item => item.Name.StartsWith("cost: ", StringComparison.OrdinalIgnoreCase))?.Name;
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

            int totalDays = 0;

            totalDays += GetWorkDaysForCostLabels(map, "S", 1);
            totalDays += GetWorkDaysForCostLabels(map, "M", 3);
            totalDays += GetWorkDaysForCostLabels(map, "L", 7);
            totalDays += GetWorkDaysForCostLabels(map, "XL", 13);

            return totalDays;
        }

        private int GetWorkDaysForCostLabels(IDictionary<string, int> map, string costLabelSuffix, int daysFactor)
        {
            int result = 0;
            if (map.TryGetValue($"cost: {costLabelSuffix}", out int numberOfItems))
            {
                result = numberOfItems * daysFactor;
            }

            return result;
        }
    }
}
