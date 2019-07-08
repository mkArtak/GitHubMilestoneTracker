using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AM.Common.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MilestoneTracker.Contracts;
using MilestoneTracker.Model;

namespace MilestoneTracker.Pages
{
    public class EditTeamModel : PageModel
    {
        private readonly ITeamsManager teamsManager;
        private readonly IUserTeamsManager userTeamsManager;
        private TeamInfo currentTeam = null;

        public TeamInfo Team { get => this.currentTeam; }

        [FromQuery(Name = QueryStringParameters.TeamName)]
        public string TeamName { get; set; }

        public EditTeamModel(ITeamsManager teamsManager, IUserTeamsManager userTeamsManager)
        {
            this.userTeamsManager = userTeamsManager.Ensure(nameof(userTeamsManager)).IsNotNull().Value;
            this.teamsManager = teamsManager.Ensure(nameof(teamsManager)).IsNotNull().Value;
        }

        public async Task<IActionResult> OnGet()
        {
            if (this.TeamName == null)
            {
                if (TempData[nameof(TeamName)] == null)
                {
                    return this.Redirect("/Teams");
                }
                else
                {
                    this.TeamName = (string)TempData[nameof(TeamName)];
                }
            }

            TeamInfo team = await this.GetCurrentTeamAsync(CancellationToken.None);
            if (team == null)
            {
                return this.Redirect("/Teams");
            }

            return Page();
        }

        private async Task<TeamInfo> GetCurrentTeamAsync(CancellationToken token)
        {
            if (this.currentTeam == null)
            {
                this.currentTeam = await this.userTeamsManager.GetTeamInfoAsync(User.Identity.Name, this.TeamName, CancellationToken.None);
            }

            return this.currentTeam;
        }
    }
}