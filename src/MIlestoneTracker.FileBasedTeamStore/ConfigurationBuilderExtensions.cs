using AM.Common.Validation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MilestoneTracker.Contracts;
using MilestoneTracker.DataManagement.Teams.Options;

namespace MilestoneTracker.DataManagement.Teams
{
    public static class ConfigurationBuilderExtensions
    {
        public static IServiceCollection AddTeams(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.Ensure(nameof(serviceCollection)).IsNotNull();
            configuration.Ensure(nameof(configuration)).IsNotNull();

            serviceCollection.AddOptions();
            serviceCollection.Configure<TeamsConfigurationOption>(configuration.GetSection("TeamsConfiguration"));
            serviceCollection.AddScoped<ITeamsManager, ConfigurationBasedTeamStore>();
            return serviceCollection;
        }
    }
}
