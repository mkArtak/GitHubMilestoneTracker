using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MilestoneTracker.Contracts;
using MilestoneTracker.Formatters;
using MilestoneTracker.Model;
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
            services.AddMvc(options =>
            {
                options.OutputFormatters.Add(new MarkdownOutputFormatter());
            });

            services.AddOptions();
            services.Configure<GitHubAuthOptions>(this.Configuration.GetSection("GitHubAuth"));

            //services.AddTeams(this.Configuration);
            services.AddTransient<ITeamsManager, InMemoryTeamsManager>();
            services.AddTransient<IUserTeamsManager, InMemoryTeamsManager>();
            services.AddGitHubMilestoneEstimator();
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = "GitHub";
            })
            .AddCookie(options =>
            {
                options.LoginPath = "/signin";
                options.LogoutPath = "/signout";
            })
            .AddGitHub(options =>
            {
                options.ClientId = this.Configuration["GitHubAuth:ClientId"];
                options.ClientSecret = this.Configuration["GitHubAuth:ClientSecret"];
                options.Scope.Add("user:email");
                options.Scope.Add("repo");
                options.SaveTokens = true;
            });
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
            app.UseAuthentication();

            app.UseMvc();
        }
    }
}
