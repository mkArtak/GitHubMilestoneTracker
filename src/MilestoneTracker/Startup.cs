using GitHub.Client;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
                //options.Conventions.AddPageRoute("/index", "home/index");
                //options.Conventions.AddPageRoute("/authorize", "home/authorize");
            });

            services.AddOptions();
            services.Configure<GitHubAuthOptions>(this.Configuration.GetSection("GitHubAuth"));
            services.AddTeams(this.Configuration);
            services.AddSingleton<WorkEstimatorFactory>();
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
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
