using AM.Common.Validation;
using GitHub.Client;
using GitHubMilestoneEstimator.Options;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MilestoneTracker.Pages
{
    public class IndexModel : PageModel
    {
        public IDictionary<string, double> Work { get; set; }

        private readonly IWorkEstimator workEstimator;

        public string[] TeamMembers { get; }

        public IndexModel(IWorkEstimator workEstimator, IOptions<GitHubOptions> gitHubOptions)
        {
            gitHubOptions.Ensure().IsNotNull();

            this.workEstimator = workEstimator.Ensure().IsNotNull().Value;
            this.TeamMembers = gitHubOptions.Value.TeamMembers;
        }

        public async Task OnGet()
        {
            IDictionary<string, double> teamWork = new ConcurrentDictionary<string, double>
            {
                ["jbagga"] = 0,
                ["NTaylorMullen"] = 0,
                ["dougbu"] = 0,
                ["ajaybhargavb"] = 0,
                ["kichalla"] = 0,
                ["javiercn"] = 0,
                ["ryanbrandenburg"] = 0,
                ["pranavkm"] = 0
            };

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
