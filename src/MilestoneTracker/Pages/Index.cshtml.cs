using AM.Common.Validation;
using GitHub.Client;
using GitHubMilestoneEstimator.Options;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MilestoneTracker.Pages
{
    public class IndexModel : PageModel
    {
        public IDictionary<string, double> Work { get; set; }

        private readonly GitHubOptions gitHubOptions;
        private readonly IWorkEstimator workEstimator;

        public string[] TeamMembers { get => this.gitHubOptions.TeamMembers; }

        public IndexModel(IWorkEstimator workEstimator, IOptions<GitHubOptions> gitHubOptions)
        {
            this.gitHubOptions = gitHubOptions.Ensure().IsNotNull().Value.Value;
            this.workEstimator = workEstimator.Ensure().IsNotNull().Value;
        }

        public async Task OnGet()
        {
            IDictionary<string, double> teamWork = new ConcurrentDictionary<string, double>(this.TeamMembers.ToDictionary(item => item, item => 0.0));

            IList<Task> tasks = new List<Task>();
            foreach (var member in teamWork)
            {
                tasks.Add(Task.Run(async () =>
                {
                    teamWork[member.Key] = await this.workEstimator.GetAmountOfWorkAsync(member.Key, "2.1.0-preview2", CancellationToken.None);
                }));
            }

            await Task.WhenAll(tasks);
            Work = teamWork;
        }
    }
}
