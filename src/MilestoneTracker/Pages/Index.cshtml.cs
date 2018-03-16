using AM.Common.Validation;
using GitHub.Client;
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
    public class IndexModel : PageModel
    {
        private const char milestoneSeparatorCharacter = ';';

        private static readonly Random random = new Random(DateTime.UtcNow.Millisecond);
        private const string AuthStateKey = "CSRF:State";
        private const string AuthTokenKey = "OAuthToken";

        private readonly TeamInfo gitHubOptions;
        private readonly GitHubAuthOptions authOptions;
        private readonly WorkEstimatorFactory workEstimatorFactory;
        private readonly GitHubClient client;
        private readonly Lazy<IEnumerable<string>> lazyMilestonesLoader;

        [FromQuery]
        public string Milestone { get; set; }

        public IEnumerable<string> Milestones { get => this.lazyMilestonesLoader.Value; }

        public WorkDataViewModel Work { get; set; }

        public string[] TeamMembers { get => this.gitHubOptions.TeamMembers; }

        public IndexModel(WorkEstimatorFactory workEstimatorFactory, TeamInfo gitHubOptions, IOptions<GitHubAuthOptions> authOptions)
        {
            this.gitHubOptions = gitHubOptions.Ensure(nameof(gitHubOptions)).IsNotNull().Value;
            this.authOptions = authOptions.Ensure(o => o.Value).IsNotNull().Value;
            this.workEstimatorFactory = workEstimatorFactory.Ensure(nameof(workEstimatorFactory)).IsNotNull().Value;

            this.client = new GitHubClient(new ProductHeaderValue(gitHubOptions.Organization), new Uri("https://github.com/"));
            this.Milestone = gitHubOptions.DefaultMilestonesToTrack;

            this.lazyMilestonesLoader = new Lazy<IEnumerable<string>>(() => this.Milestone?
                .Split(milestoneSeparatorCharacter, StringSplitOptions.RemoveEmptyEntries)
                .Select(item => item.Trim()));
        }

        public async Task<IActionResult> OnGet()
        {
            if (this.Milestones != null)
            {
                IWorkEstimator workEstimator = GetWorkEstimator();
                if (workEstimator == null)
                {
                    return Redirect(this.GetOauthLoginUrl());
                }

                try
                {
                    await this.RetrieveWorkloadAsync(workEstimator);
                }
                catch (AuthorizationException)
                {
                    return Redirect(this.GetOauthLoginUrl());
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
                var token = await client.Oauth.CreateAccessToken(
                    new OauthTokenRequest(this.authOptions.ClientId, this.authOptions.ClientSecret, code));
                TempData[AuthTokenKey] = token.AccessToken;
            }

            return RedirectToAction("Index");
        }

        private async Task RetrieveWorkloadAsync(IWorkEstimator workEstimator)
        {
            this.Work = new WorkDataViewModel();

            IList<Task> tasks = new List<Task>();
            using (CancellationTokenSource tokenSource = new CancellationTokenSource())
            {
                foreach (var member in this.TeamMembers)
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

        private IWorkEstimator GetWorkEstimator()
        {
            var accessToken = TempData.Peek(AuthTokenKey) as string;
            // This allows the client to make requests to the GitHub API on behalf of the user
            // without ever having the user's OAuth credentials.
            return accessToken == null ? null : this.workEstimatorFactory.Create(accessToken);
        }

        private string GetOauthLoginUrl()
        {
            string csrf = GenerateRandomString(24);
            TempData[AuthStateKey] = csrf;

            // 1. Redirect users to request GitHub access
            var request = new OauthLoginRequest(this.authOptions.ClientId)
            {
                Scopes = { "user", "notifications" },
                State = csrf,
            };
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
    }
}
