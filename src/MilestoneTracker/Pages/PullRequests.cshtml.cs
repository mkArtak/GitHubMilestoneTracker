using AM.Common.Validation;
using GitHub.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MilestoneTracker.Contracts;
using MilestoneTracker.Model;
using MilestoneTracker.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MilestoneTracker.Pages
{
    [Authorize]
    public class PullRequestsModel : PageModel
    {
        private const char milestoneSeparatorCharacter = ';';

        private readonly WorkEstimatorFactory workEstimatorFactory;
        private readonly Lazy<IEnumerable<string>> lazyMilestonesLoader;
        private readonly Lazy<IEnumerable<string>> lazyLabelsLoader;
        private readonly IUserTeamsManager userTeamsManager;
        private TeamInfo currentTeam = null;
        private string milestone;

        [FromQuery(Name = QueryStringParameters.Milestone)]
        public string Milestone
        {
            get => this.milestone ?? this.currentTeam?.DefaultMilestonesToTrack;
            set => this.milestone = value;
        }

        [FromQuery(Name = QueryStringParameters.TeamName)]
        public string TeamName { get; set; }

        [FromQuery]
        public string Label { get; set; }

        public IEnumerable<string> Labels { get => this.lazyLabelsLoader.Value; }

        public IEnumerable<string> Milestones { get => this.lazyMilestonesLoader.Value; }

        public MergedPRsViewModel PRVM { get; set; }

        public PullRequestsModel(
            WorkEstimatorFactory workEstimatorFactory,
            IUserTeamsManager userTeamsManager)
        {
            this.workEstimatorFactory = workEstimatorFactory.Ensure(nameof(workEstimatorFactory)).IsNotNull().Value;
            this.userTeamsManager = userTeamsManager.Ensure(nameof(userTeamsManager)).IsNotNull().Value;

            this.lazyMilestonesLoader = new Lazy<IEnumerable<string>>(() =>
            {
                return this.Milestone?
                    .Split(milestoneSeparatorCharacter, StringSplitOptions.RemoveEmptyEntries)
                    .Select(item => item.Trim());
            });

            this.lazyLabelsLoader = new Lazy<IEnumerable<string>>(() =>
            {
                if (this.Label == null)
                    return null;

                return this.Label.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(item => item.Trim()).ToArray();
            });
        }

        public async Task<IActionResult> OnGet(CancellationToken cancellationToken)
        {
            if (this.TeamName == null)
            {
                return Redirect("/Teams");
            }

            if (this.currentTeam == null)
            {
                await this.GetCurrentTeamAsync(cancellationToken);
            }

            if (this.Milestones != null)
            {
                TeamInfo team = await this.GetCurrentTeamAsync(cancellationToken);
                if (team == null)
                {
                    return Redirect("/Teams");
                }

                IWorkEstimator workEstimator = await this.GetWorkEstimatorAsync(team, cancellationToken);

                await this.RetrievePullRequests(workEstimator, team, cancellationToken);
            }

            return Page();
        }

        private async Task RetrievePullRequests(IWorkEstimator workEstimator, TeamInfo team, CancellationToken cancellationToken)
        {
            this.PRVM = new MergedPRsViewModel
            {
                PullRequests = await workEstimator.GetPullRequestsAsync(DateTimeOffset.UtcNow.AddDays(-7), cancellationToken),
                Team = team,
                IconRetriever = await this.CreateProfileIconRetriever(team, cancellationToken)
            };
        }

        private async Task<IWorkEstimator> GetWorkEstimatorAsync(TeamInfo team, CancellationToken cancellationToken)
        {
            string accessToken = await GetAccessToken();

            // This allows the client to make requests to the GitHub API on behalf of the user
            // without ever having the user's OAuth credentials.
            return this.workEstimatorFactory.Create(accessToken, team);
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

        private async Task<TeamInfo> GetCurrentTeamAsync(CancellationToken token)
        {
            if (this.currentTeam == null)
            {
                this.currentTeam = await this.userTeamsManager.GetTeamInfoAsync(User.Identity.Name, this.TeamName, token);
            }

            return this.currentTeam;
        }
    }
}
