using AM.Common.Validation;
using GitHub.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MilestoneTracker.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MilestoneTracker.Controllers
{
    [Route("api/Burndown")]
    [Authorize]
    public class BurndownController : Controller
    {
        private readonly WorkEstimatorFactory workEstimatorFactory;
        private readonly IUserTeamsManager userTeamsManager;
        private readonly ITeamsManager teamsManager;
        private TeamInfo currentTeam;

        public BurndownController(
            WorkEstimatorFactory workEstimatorFactory,
            ITeamsManager teamsManager,
            IUserTeamsManager userTeamsManager)
        {
            this.workEstimatorFactory = workEstimatorFactory.Ensure(nameof(workEstimatorFactory)).IsNotNull().Value;
            this.userTeamsManager = userTeamsManager.Ensure(nameof(userTeamsManager)).IsNotNull().Value;
            this.teamsManager = teamsManager.Ensure(nameof(teamsManager)).IsNotNull().Value;
        }

        [HttpGet]
        public async Task<IActionResult> GetBurndownDataAsync([FromQuery]string teamName, [FromQuery]string milestone)
        {
            ///TODO: Remove this as it's a workaroudn of a problem
            ///
            milestone = "2.1.0-preview2";
            if (!ModelState.IsValid)
            {
                throw new InvalidOperationException(ModelState.Values.First().Errors.First().ErrorMessage);
            }

            IWorkEstimator workEstimator = await GetWorkEstimatorAsync(CancellationToken.None);

            TeamInfo team = await this.GetCurrentTeamAsync(CancellationToken.None);
            IEnumerable<WorkDTO> burnDownDatas = await workEstimator.GetBurndownDataAsync(team, milestone, CancellationToken.None);

            return new JsonResult(burnDownDatas);
        }

        private async Task<IWorkEstimator> GetWorkEstimatorAsync(CancellationToken cancellationToken)
        {
            string accessToken = await this.HttpContext.GetTokenAsync("access_token");

            // This allows the client to make requests to the GitHub API on behalf of the user
            // without ever having the user's OAuth credentials.
            return accessToken == null ? null : this.workEstimatorFactory.Create(accessToken, await this.GetCurrentTeamAsync(cancellationToken));
        }

        private async Task<TeamInfo> GetCurrentTeamAsync(CancellationToken token)
        {
            if (this.currentTeam == null)
            {
                // TODO: Fix: Using only the first team for now for simplicity
                IEnumerable<string> teams = await this.userTeamsManager.GetUserTeamsAsync(User.Identity.Name, CancellationToken.None);
                this.currentTeam = await this.teamsManager.GetTeamInfoAsync(teams.First(), token);
            }

            return this.currentTeam;
        }
    }
}