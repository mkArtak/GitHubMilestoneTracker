using GitHub.Client;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MilestoneTracker.Pages
{
    public class IndexModel : PageModel
    {
        public IDictionary<string, int> Work { get; set; }

        public async Task OnGet()
        {
            IDictionary<string, int> teamWork = new ConcurrentDictionary<string, int>
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
            IWorkEstimator estimator = new WorkEstimatorFactory("aspnet").Create();
            foreach (var member in teamWork)
            {
                tasks.Add(Task.Run(async () =>
                {
                    teamWork[member.Key] = await estimator.GetAmountOfWorkAsync(member.Key, "2.1.0-preview2", CancellationToken.None);
                }));
            }

            await Task.WhenAll(tasks);
            Work = teamWork;
        }
    }
}
