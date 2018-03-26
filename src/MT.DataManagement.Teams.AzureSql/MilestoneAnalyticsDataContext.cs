using Microsoft.EntityFrameworkCore;
using MT.DataManagement.Teams.AzureSql.Model;

namespace MT.DataManagement.Teams.AzureSql
{
    public class MilestoneAnalyticsDataContext : DbContext
    {
        public DbSet<Team> Teams { get; set; }

        public MilestoneAnalyticsDataContext(DbContextOptions<MilestoneAnalyticsDataContext> options) : base(options)
        {
        }
    }
}
