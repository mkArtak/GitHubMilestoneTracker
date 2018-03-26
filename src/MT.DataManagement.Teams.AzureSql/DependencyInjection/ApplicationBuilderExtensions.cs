using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MilestoneTracker.Contracts;

namespace MT.DataManagement.Teams.AzureSql.DependencyInjection
{
    public static class ApplicationBuilderExtensions
    {
        public static IServiceCollection AddTeams(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddDbContext<MilestoneAnalyticsDataContext>(options => options.UseSqlServer(configuration.GetConnectionString("teamsDbConnection")));
            serviceCollection.AddScoped<ITeamsManager, TeamsManager>();

            return serviceCollection;
        }
    }
}
