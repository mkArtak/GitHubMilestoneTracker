using GitHub.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MilestoneTracker.Contracts;
using MilestoneTracker.DataManagement.Teams;
using MilestoneTracker.Options;

namespace MilestoneTracker
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().AddRazorPagesOptions(options =>
            {
                options.Conventions.AddPageRoute("/index", "home/index");
                options.Conventions.AddPageRoute("/authorize", "home/authorize");
            });

            services.AddOptions();
            services.Configure<GitHubAuthOptions>(this.Configuration.GetSection("GitHubAuth"));
            services.AddTeams(this.Configuration);
            services.AddSingleton<WorkEstimatorFactory>();

            #region Authentication config
            //services.AddAuthentication(options =>
            // {
            //     options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //     options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //     options.DefaultChallengeScheme = "GitHub";
            // })
            //    .AddCookie(options =>
            //    {
            //        options.LoginPath = new PathString("/signin");
            //    })
            //    .AddOAuth("GitHub", options =>
            //    {
            //        options.CallbackPath = new PathString("/signin-github");
            //        options.AuthorizationEndpoint = "https://github.com/login/oauth/authorize";
            //        options.TokenEndpoint = "https://github.com/login/oauth/access_token";
            //        options.UserInformationEndpoint = "https://api.github.com/user";
            //        options.ClaimsIssuer = "GitHub";

            //        options.ClientId = Configuration["GitHubAuth::ClientId"];
            //        options.ClientSecret = Configuration["GitHubAuth::ClientSecret"];
            //        //options.Scope.Add("repo");
            //        options.SaveTokens = true;

            //        options.Events.OnCreatingTicket = async context =>
            //        {
            //            var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
            //            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);

            //            var response = await context.Backchannel.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, context.HttpContext.RequestAborted);
            //            response.EnsureSuccessStatusCode();

            //            var payload = JObject.Parse(await response.Content.ReadAsStringAsync());

            //            // Add GitHub claims
            //            context.Identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, payload.Value<string>("id"), context.Options.ClaimsIssuer));
            //            context.Identity.AddClaim(new Claim(ClaimTypes.Name, payload.Value<string>("login"), context.Options.ClaimsIssuer));
            //            context.Identity.AddClaim(new Claim("urn:github:url", payload.Value<string>("url"), context.Options.ClaimsIssuer));
            //        };
            //    }); 
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            app.UseMvc();
        }
    }
}
