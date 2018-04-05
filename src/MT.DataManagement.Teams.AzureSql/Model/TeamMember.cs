using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MT.DataManagement.Teams.AzureSql.Model
{
    [Table("TeamMembers")]
    public class TeamMember : IJoinEntity<Team>, IJoinEntity<Member>
    {
        [Key]
        public string TeamId { get; set; }
        public Team Team { get; set; }
        Team IJoinEntity<Team>.Navigation { get => this.Team; set => this.Team = value; }

        [Key]
        public string MemberId { get; set; }
        public Member Member { get; set; }
        Member IJoinEntity<Member>.Navigation { get => this.Member; set => this.Member = value; }

        public bool IncludeInReports { get; set; }
    }
}
