using AM.Common.Validation;
using GitHub.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MilestoneTracker.Contracts;
using MilestoneTracker.ViewModels;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MilestoneTracker.Controllers
{
    [Route("api/pulls")]
    [Authorize]
    public class PullRequestsController : ControllerBase
    {
        private readonly WorkEstimatorFactory workEstimatorFactory;
        private readonly ITeamsManager teamsManager;
        private TeamInfo currentTeam;

        public PullRequestsController(
            WorkEstimatorFactory workEstimatorFactory,
            ITeamsManager teamsManager)
        {
            this.workEstimatorFactory = workEstimatorFactory.Ensure(nameof(workEstimatorFactory)).IsNotNull().Value;
            this.teamsManager = teamsManager.Ensure(nameof(teamsManager)).IsNotNull().Value;
        }

        [HttpGet("{team}")]
        [Produces("text/markdown")]
        public async Task<ActionResult<MergedPRsViewModel>> ExportMarkdown(string team, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                throw new InvalidOperationException(ModelState.Values.First().Errors.First().ErrorMessage);
            }

            IWorkEstimator workEstimator = await GetWorkEstimatorAsync(team, cancellationToken);
            TeamInfo teamInfo = await this.GetCurrentTeamAsync(team, cancellationToken);

            return new MergedPRsViewModel
            {
                PullRequests = await workEstimator.GetPullRequestsAsync(DateTimeOffset.UtcNow.AddDays(-7), cancellationToken),
                Team = teamInfo,
                IconRetriever = await this.CreateProfileIconRetriever(teamInfo, cancellationToken)
            };
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

        private async Task<IProfileIconRetriever> CreateProfileIconRetriever(TeamInfo team, CancellationToken cancellationToken)
        {
            string accessToken = await GetAccessToken();

            // This allows the client to make requests to the GitHub API on behalf of the user
            // without ever having the user's OAuth credentials.
            return this.workEstimatorFactory.CreateProfileIconRetriever(accessToken, team);
        }

        private Task<string> GetAccessToken()
        {
            return this.HttpContext.GetTokenAsync("access_token");
        }
    }
}