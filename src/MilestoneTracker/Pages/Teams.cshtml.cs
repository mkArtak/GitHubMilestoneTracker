using AM.Common.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MilestoneTracker.Contracts;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MilestoneTracker.Pages
{
    public class TeamsModel : PageModel
    {
        private readonly ITeamsManager teamsManager;
        private readonly IUserTeamsManager userTeamsManager;

        public IEnumerable<string> Teams { get; private set; }

        public TeamsModel(ITeamsManager teamsManager, IUserTeamsManager userTeamsManager)
        {
            this.teamsManager = teamsManager.Ensure(nameof(teamsManager)).IsNotNull().Value;
            this.userTeamsManager = userTeamsManager.Ensure(nameof(userTeamsManager)).IsNotNull().Value;
        }

        [Authorize]
        public async Task<IActionResult> OnGet()
        {
            IEnumerable<string> teams = await this.userTeamsManager.GetUserTeamsAsync(User.Identity.Name, CancellationToken.None);
            this.Teams = teams;
            return Page();
        }
    }
}