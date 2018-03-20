using AM.Common.Validation;
using GitHub.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using MilestoneTracker.Contracts;
using MilestoneTracker.Model;
using MilestoneTracker.Options;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MilestoneTracker.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private const char milestoneSeparatorCharacter = ';';

        private static readonly Random random = new Random(DateTime.UtcNow.Millisecond);
        private const string AuthStateKey = "CSRF:State";
        private const string AuthTokenKey = "OAuthToken";

        private readonly GitHubAuthOptions authOptions;
        private readonly WorkEstimatorFactory workEstimatorFactory;
        private readonly Lazy<IEnumerable<string>> lazyMilestonesLoader;
        private readonly ITeamsManager teamsManager;
        private readonly IUserTeamsManager userTeamsManager;
        private TeamInfo currentTeam = null;

        [FromQuery]
        public string Milestone { get; set; }

        public IEnumerable<string> Milestones { get => this.lazyMilestonesLoader.Value; }

        public WorkDataViewModel Work { get; set; }

        public IndexModel(
            WorkEstimatorFactory workEstimatorFactory,
            ITeamsManager teamsManager,
            IUserTeamsManager userTeamsManager,
            IOptions<GitHubAuthOptions> authOptions)
        {
            this.authOptions = authOptions.Ensure(o => o.Value).IsNotNull().Value;
            this.workEstimatorFactory = workEstimatorFactory.Ensure(nameof(workEstimatorFactory)).IsNotNull().Value;
            this.userTeamsManager = userTeamsManager.Ensure(nameof(userTeamsManager)).IsNotNull().Value;

            this.teamsManager = teamsManager.Ensure(nameof(teamsManager)).IsNotNull().Value;

            this.lazyMilestonesLoader = new Lazy<IEnumerable<string>>(() => this.Milestone?
                .Split(milestoneSeparatorCharacter, StringSplitOptions.RemoveEmptyEntries)
                .Select(item => item.Trim()));
        }

        public async Task<IActionResult> OnGet()
        {
            if (this.Milestones != null)
            {
                IWorkEstimator workEstimator = await this.GetWorkEstimatorAsync(CancellationToken.None);
                if (workEstimator == null)
                {
                    return Redirect(await this.GetOauthLoginUrlAsync(CancellationToken.None));
                }

                try
                {
                    await this.RetrieveWorkloadAsync(workEstimator, CancellationToken.None);
                }
                catch (AuthorizationException)
                {
                    return Redirect(await this.GetOauthLoginUrlAsync(CancellationToken.None));
                }
                catch (Exception ex)
                {
                    ;
                }
            }

            return Page();
        }

        [HttpGet]
        // This is the Callback URL that the GitHub OAuth Login page will redirect back to.
        public async Task<IActionResult> OnGetAuthorize(string code, string state)
        {
            if (!String.IsNullOrEmpty(code))
            {
                var expectedState = TempData.Peek(AuthStateKey) as string;
                if (state != expectedState)
                {
                    throw new InvalidOperationException("Security failure!");
                }

                TempData[AuthStateKey] = null;
                GitHubClient client = await this.GetClientAsync(CancellationToken.None);
                var token = await client.Oauth.CreateAccessToken(
                    new OauthTokenRequest(this.authOptions.ClientId, this.authOptions.ClientSecret, code));
                TempData[AuthTokenKey] = token.AccessToken;
            }

            return RedirectToAction("Index");
        }

        private async Task RetrieveWorkloadAsync(IWorkEstimator workEstimator, CancellationToken cancellationToken)
        {
            this.Work = new WorkDataViewModel();

            IList<Task> tasks = new List<Task>();
            using (CancellationTokenSource tokenSource = new CancellationTokenSource())
            {
                TeamInfo currentTeam = await this.GetCurrentTeamAsync(cancellationToken);
                foreach (var member in currentTeam.TeamMembers)
                {
                    foreach (var milestone in this.Milestones)
                    {
                        tasks.Add(Task.Run(async () =>
                        {
                            if (!tokenSource.IsCancellationRequested)
                            {
                                this.Work[member, milestone] = await workEstimator.GetAmountOfWorkAsync(member, milestone, tokenSource.Token);
                            }
                        }));
                    }
                }

                await Task.WhenAll(tasks);
            }
        }

        private async Task<IWorkEstimator> GetWorkEstimatorAsync(CancellationToken cancellationToken)
        {
            //var accessToken = TempData.Peek(AuthTokenKey) as string;
            string accessToken = await HttpContext.GetTokenAsync("access_token");

            // This allows the client to make requests to the GitHub API on behalf of the user
            // without ever having the user's OAuth credentials.
            return accessToken == null ? null : this.workEstimatorFactory.Create(accessToken, await this.GetCurrentTeamAsync(cancellationToken));
        }

        private async Task<string> GetOauthLoginUrlAsync(CancellationToken cancellationToken)
        {
            string csrf = GenerateRandomString(24);
            TempData[AuthStateKey] = csrf;

            // 1. Redirect users to request GitHub access
            var request = new OauthLoginRequest(this.authOptions.ClientId)
            {
                Scopes = { "user", "notifications" },
                State = csrf,
            };
            GitHubClient client = await this.GetClientAsync(cancellationToken);
            var oauthLoginUrl = client.Oauth.GetGitHubLoginUrl(request);
            return oauthLoginUrl.ToString();
        }

        private static string GenerateRandomString(int length)
        {
            var passChars = new char[length];
            var characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_";
            for (int i = 0; i < length; i++)
            {
                passChars[i] = characters[random.Next(0, characters.Length - 1)];
            }

            return new string(passChars);
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

        private async Task<GitHubClient> GetClientAsync(CancellationToken cancellationToken)
        {
            TeamInfo currentTeam = await this.GetCurrentTeamAsync(cancellationToken);
            return new GitHubClient(new ProductHeaderValue(currentTeam.Organization), new Uri("https://github.com/"));
        }
    }
}
