using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MilestoneTracker.Contracts;
using MT.DataManagement.Teams.AzureSql;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTeams(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddDbContext<MilestoneAnalyticsDataContext>(options => options.UseSqlServer(configuration.GetConnectionString("teamsDbConnection")));
            serviceCollection.AddScoped<ITeamsManager, TeamsManager>();
            serviceCollection.AddScoped<IUserTeamsManager, TeamsManager>();

            return serviceCollection;
        }
    }
}
