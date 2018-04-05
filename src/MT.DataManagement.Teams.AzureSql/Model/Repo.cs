using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MT.DataManagement.Teams.AzureSql.Model
{
    public class Repo
    {
        [Key]
        public string RepoId { get; set; }

        private ICollection<TeamRepo> TeamRepos { get; set; } = new List<TeamRepo>();

        [NotMapped]
        public IEnumerable<Team> Teams { get; }

        public Repo()
        {
            this.Teams = new JoinCollectionFacade<Team, Repo, TeamRepo>(this, TeamRepos);
        }
    }
}
