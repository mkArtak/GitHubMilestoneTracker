using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MT.DataManagement.Teams.AzureSql.Model
{
    public class Team
    {
        [Key]
        public string TeamId { get; set; }

        public string DefaultMilestonesToTrack { get; set; }

        public string Organization { get; set; }

        private ICollection<TeamMember> TeamMembers { get; set; } = new List<TeamMember>();
        private ICollection<TeamRepo> TeamRepos { get; set; } = new List<TeamRepo>();
        public ICollection<CostMarker> CostMarkers { get; set; } = new List<CostMarker>();

        [NotMapped]
        public ICollection<Member> Members { get; }

        [NotMapped]
        public ICollection<Repo> Repos { get; }

        public Team()
        {
            this.Members = new JoinCollectionFacade<Member, Team, TeamMember>(this, TeamMembers);
            this.Repos = new JoinCollectionFacade<Repo, Team, TeamRepo>(this, TeamRepos);
        }
    }
}
