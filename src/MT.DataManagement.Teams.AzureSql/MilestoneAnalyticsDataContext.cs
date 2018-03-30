using Microsoft.EntityFrameworkCore;
using MT.DataManagement.Teams.AzureSql.Model;

namespace MT.DataManagement.Teams.AzureSql
{
    public class MilestoneAnalyticsDataContext : DbContext
    {
        public DbSet<Team> Teams { get; set; }

        public DbSet<Member> Members { get; set; }

        public MilestoneAnalyticsDataContext(DbContextOptions<MilestoneAnalyticsDataContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Team>()
                .HasKey(t => t.TeamId);

            modelBuilder.Entity<Team>()
                .HasMany(t => t.Members);

            modelBuilder.Entity<Team>()
                .HasMany(t => t.CostMarkers);
        }
    }
}
