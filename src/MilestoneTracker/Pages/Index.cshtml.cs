using AM.Common.Validation;
using GitHub.Client;
using GitHubMilestoneEstimator.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MilestoneTracker.Model;
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
        private const char milestoneSeparatorCharacter = ';';

        private readonly GitHubOptions gitHubOptions;
        private readonly IWorkEstimator workEstimator;

        [FromQuery]
        public string Milestone { get; set; }

        public IEnumerable<string> Milestones { get => this.Milestone?.Split(milestoneSeparatorCharacter); }

        public WorkDataViewModel Work { get; set; }

        public string[] TeamMembers { get => this.gitHubOptions.TeamMembers; }

        public IndexModel(IWorkEstimator workEstimator, GitHubOptions gitHubOptions)
        {
            this.gitHubOptions = gitHubOptions.Ensure(nameof(gitHubOptions)).IsNotNull().Value;
            this.workEstimator = workEstimator.Ensure(nameof(workEstimator)).IsNotNull().Value;
        }

        public async Task OnGet()
        {
            if (this.Milestones != null)
            {
                await this.RetrieveWorkloadAsync();
            }
        }

        private async Task RetrieveWorkloadAsync()
        {
            this.Work = new WorkDataViewModel();

            IList<Task> tasks = new List<Task>();
            using (CancellationTokenSource tokenSource = new CancellationTokenSource())
            {
                foreach (var member in this.TeamMembers)
                {
                    foreach (var milestone in this.Milestones)
                    {
                        tasks.Add(Task.Run(async () =>
                        {
                            if (!tokenSource.IsCancellationRequested)
                            {
                                this.Work[member, milestone] = await this.workEstimator.GetAmountOfWorkAsync(member, milestone, tokenSource.Token);
                            }
                        }));
                    }
                }

                await Task.WhenAll(tasks);
            }
        }
    }
}
