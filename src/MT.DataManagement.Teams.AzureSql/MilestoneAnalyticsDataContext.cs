using Microsoft.EntityFrameworkCore;
using MT.DataManagement.Teams.AzureSql.Model;
using System.ComponentModel.DataAnnotations.Schema;

namespace MT.DataManagement.Teams.AzureSql
{
    public class MilestoneAnalyticsDataContext : DbContext
    {
        public DbSet<Team> Teams { get; set; }

        public DbSet<Member> Members { get; set; }

        public DbSet<Repo> Repos { get; set; }

        public DbSet<TeamMember> TeamMembers { get; set; }

        public DbSet<TeamRepo> TeamRepos { get; set; }

        public MilestoneAnalyticsDataContext(DbContextOptions<MilestoneAnalyticsDataContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Team>().HasMany(t => t.CostMarkers);

            modelBuilder.Entity<CostMarker>().HasKey(cm => new { cm.CostMarkerId, cm.TeamId });

            modelBuilder.Entity<TeamRepo>().HasKey(tr => new { tr.TeamId, tr.RepoId });
            modelBuilder.Entity<TeamRepo>().HasOne(tr => tr.Team).WithMany("TeamRepos");
            modelBuilder.Entity<TeamRepo>().HasOne(tr => tr.Repo).WithMany("TeamRepos");

            modelBuilder.Entity<TeamMember>().HasKey(tm => new { tm.TeamId, tm.MemberId });
            modelBuilder.Entity<TeamMember>().HasOne(tm => tm.Member).WithMany("TeamMembers");
            modelBuilder.Entity<TeamMember>().HasOne(tm => tm.Team).WithMany("TeamMembers");
        }
    }
}
