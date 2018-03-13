using AM.Common.Validation;
using GitHub.Client;
using GitHubMilestoneEstimator.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MilestoneTracker.Pages
{
    public class IndexModel : PageModel
    {
        private readonly GitHubOptions gitHubOptions;
        private readonly IWorkEstimator workEstimator;

        [FromQuery]
        public string Milestone { get; set; }

        public IDictionary<string, double> Work { get; set; }

        public string[] TeamMembers { get => this.gitHubOptions.TeamMembers; }

        public IndexModel(IWorkEstimator workEstimator, GitHubOptions gitHubOptions)
        {
            this.gitHubOptions = gitHubOptions.Ensure(nameof(gitHubOptions)).IsNotNull().Value;
            this.workEstimator = workEstimator.Ensure(nameof(workEstimator)).IsNotNull().Value;
        }

        public async Task OnGet()
        {
            if (!String.IsNullOrWhiteSpace(this.Milestone))
            {
                IDictionary<string, double> teamWork = await this.RetrieveWorkloadAsync();
                Work = teamWork;
            }
        }

        private async Task<IDictionary<string, double>> RetrieveWorkloadAsync()
        {
            IDictionary<string, double> teamWork = new ConcurrentDictionary<string, double>(this.TeamMembers.ToDictionary(item => item, item => 0.0));

            IList<Task> tasks = new List<Task>();
            using (CancellationTokenSource tokenSource = new CancellationTokenSource())
            {
                foreach (var member in teamWork)
                {
                    tasks.Add(Task.Run(async () =>
                    {
                        if (!tokenSource.IsCancellationRequested)
                        {
                            teamWork[member.Key] = await this.workEstimator.GetAmountOfWorkAsync(member.Key, this.Milestone, tokenSource.Token);
                        }
                    }));
                }

                await Task.WhenAll(tasks);
            }

            return teamWork;
        }
    }
}
