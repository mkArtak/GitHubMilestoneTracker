using AM.Common.Validation;
using GitHub.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MilestoneTracker.Contracts;
using MilestoneTracker.Contracts.DTO;
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
        public async Task<IActionResult> GetBurndownDataAsync([FromQuery]string teamName, [FromQuery]string milestone, [FromQuery]string label, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                throw new InvalidOperationException(ModelState.Values.First().Errors.First().ErrorMessage);
            }

            IWorkEstimator workEstimator = await GetWorkEstimatorAsync(teamName, CancellationToken.None);

            TeamInfo team = await this.GetCurrentTeamAsync(teamName, CancellationToken.None);
            IEnumerable<string> labels = null;
            if (label != null)
            {
                labels = label.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(item => item.Trim()).ToArray();
            }

            BurndownDTO burnDownData = await workEstimator.GetBurndownDataAsync(
                new IssuesQuery
                {
                    Team = team,
                    Milestone = milestone,
                    FilterLabels = labels
                }, cancellationToken);

            return new JsonResult(burnDownData);
        }

        private async Task<IWorkEstimator> GetWorkEstimatorAsync(string teamName, CancellationToken cancellationToken)
        {
            string accessToken = await this.HttpContext.GetTokenAsync("access_token");

            // This allows the client to make requests to the GitHub API on behalf of the user
            // without ever having the user's OAuth credentials.
            return accessToken == null ? null : this.workEstimatorFactory.Create(accessToken, await this.GetCurrentTeamAsync(teamName, cancellationToken));
        }

        private async Task<TeamInfo> GetCurrentTeamAsync(string teamName, CancellationToken token)
        {
            if (this.currentTeam == null)
            {
                this.currentTeam = await this.teamsManager.GetTeamInfoAsync(teamName, token);
            }

            return this.currentTeam;
        }
    }
}