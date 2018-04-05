using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MT.DataManagement.Teams.AzureSql.Model
{
    public class Member
    {
        [Key]
        public string MemberId { get; set; }

        private ICollection<TeamMember> TeamMembers { get; set; } = new List<TeamMember>();

        [NotMapped]
        public IEnumerable<Team> Teams { get; }

        public Member()
        {
            this.Teams = new JoinCollectionFacade<Team, Member, TeamMember>(this, this.TeamMembers);
        }
    }
}
