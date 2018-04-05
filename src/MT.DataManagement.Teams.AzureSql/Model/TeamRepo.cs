using System.ComponentModel.DataAnnotations;

namespace MT.DataManagement.Teams.AzureSql.Model
{
    public class TeamRepo : IJoinEntity<Repo>, IJoinEntity<Team>
    {
        [Key]
        public string RepoId { get; set; }
        public Repo Repo { get; set; }
        Repo IJoinEntity<Repo>.Navigation { get => this.Repo; set => this.Repo = value; }

        [Key]
        public string TeamId { get; set; }
        public Team Team { get; set; }
        Team IJoinEntity<Team>.Navigation { get => this.Team; set => this.Team = value; }
    }
}
