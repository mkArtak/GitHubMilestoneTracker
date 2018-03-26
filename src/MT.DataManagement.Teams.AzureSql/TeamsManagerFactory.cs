using Microsoft.EntityFrameworkCore;
using MilestoneTracker.Contracts;

namespace MT.DataManagement.Teams.AzureSql
{
    public class TeamsManagerFactory
    {
        public ITeamsManager Create(DbContextOptions options)
        {
            return new TeamsManager(new MilestoneAnalyticsDataContext(options));
        }
    }
}
